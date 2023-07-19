#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using System.Collections;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     Provides the core functionality for running a MarkDialogue script. 
    ///     Inherit from this class to provide your own implementation as require by your project. 
    /// </summary>
    public abstract class BaseMarkDialoguePlayer : MonoBehaviour
    {
        /// <summary>The script collection containing the MarkDialogue script we want to run.</summary>
        [field: SerializeField] public MarkDialogueScriptCollection? ScriptCollection { get; set; }

        /// <summary>
        ///     The name of the MarkDialogue script to run. 
        ///     This defaults to the default script. Not used if <see cref="ScriptCollection"/> only contains one script.
        /// </summary>
        [field: SerializeField] public string ScriptToRun { get; set; } = MarkDialogueScript.DEFAULT_SCRIPT_NAME;

        /// <summary>
        ///     <para>
        ///         If <see langword="true"/>, this dialogue is kicked off the moment this component's <see cref="Start"/> method is invoked.
        ///     </para>
        ///     <para>
        ///         If <see langword="false"/>, you'll need to call <see cref="PlayScript"/> from your own code.
        ///     </para>
        /// </summary>
        [field: SerializeField] public bool AutoStart { get; set; } = false;

        protected virtual void Start()
        {
            if (AutoStart)
            {
                PlayScript();
            }
        }

        /// <summary>
        ///     Starts the script from the beginning.
        /// </summary>
        public virtual void PlayScript()
        {
            if (ScriptCollection == null)
            {
                Debug.LogError($"Could not play MarkDialogue script - Supplied {nameof(ScriptCollection)} was null.");
                return;
            }

            if (ScriptCollection.Scripts.Count == 0)
            {
                Debug.LogError($"Could not run script {ScriptToRun} in collection {ScriptCollection} - The supplied collection has no scripts! Maybe you need to reimport?");
                return;
            }

            var script = ScriptCollection.GetDialogueScript(ScriptToRun);
            if (script == null)
            {
                throw new MissingMarkDialogueScriptException(ScriptCollection, ScriptToRun ?? "<null>");
            }

            StartCoroutine(DialoguePlaybackLoop(ScriptCollection, script));
        }

        private IEnumerator DialoguePlaybackLoop(MarkDialogueScriptCollection collection, MarkDialogueScript script)
        {
            var state = new MarkDialoguePlayerState(this, collection, script);

            OnDialogueStart(state);

            for(; !state.IsComplete && state.CurrentScriptLineNumber <= state.Script.Lines.Count; ++state.CurrentScriptLineNumber)
            {
                if (state.CurrentScriptLineNumber >= state.Script.Lines.Count)
                {
                    if (state.Choices.Count == 0)
                    {
                        // Nowhere to go. Dialogue ends.
                        state.IsComplete = true;
                        break;
                    }

                    var selectedChoice = state.Choices[0];
                    if (state.Choices.Count > 1)
                    {
                        var continuation = new WaitForChoicesContinuation(state.Choices);
                        OnDialogueChoices(continuation);
                        yield return continuation;
                        selectedChoice = continuation.SelectedChoice;

                        if (selectedChoice == null)
                        {
                            throw new InvalidOperationException($"Expected {nameof(WaitForChoicesContinuation)} to return a choice, but got null instead!");
                        }
                    }

                    if (selectedChoice.TargetScript[0] == '#') // Is local jump
                    {
                        var internalScriptName = selectedChoice.TargetScript.Substring(1);
                        var newScript = state.Collection.GetDialogueScript(internalScriptName);
                        if (newScript == null)
                        {
                            throw new MissingMarkDialogueScriptException(state.Collection, internalScriptName);
                        }
                        state.StartNewScript(newScript);
                    }
                    else
                    {
                        // TODO: Parse selectedChoice
                        Debug.Log($"TODO: Jump to {selectedChoice.TargetScript} (Display name: {selectedChoice.DisplayName}");
                        break;
                    }
                }

                var scriptLine = state.Script.Lines[state.CurrentScriptLineNumber];

                switch (scriptLine.type)
                {
                    case MarkDialogueScriptLineType.Character:
                        state.CurrentCharacter = MarkDialogueCharacter.FromScriptLine(scriptLine);
                        break;

                    case MarkDialogueScriptLineType.Dialogue:
                        if (state.CurrentCharacter == null)
                        {
                            throw new MissingMarkDialogueCharacterException(collection, script, scriptLine);
                        }

                        var dialogueLine = MarkDialogueLine.FromScriptLine(scriptLine);
                        var continuator = new WaitForDialogueContinuation();
                        OnDialogueLine(state.CurrentCharacter, dialogueLine, continuator);
                        yield return continuator;
                        break;

                    case MarkDialogueScriptLineType.Link:
                        var link = MarkDialogueLink.FromScriptLine(scriptLine);
                        state.Choices.Add(link);
                        break;

                    case MarkDialogueScriptLineType.Quote:
                        var quoteLine = MarkDialogueLine.FromScriptLine(scriptLine);
                        quoteLine.LineText = quoteLine.LineText.Substring(1).Trim(); // Remove the leading '>' character.
                        OnQuoteText(quoteLine);
                        break;

                    case MarkDialogueScriptLineType.Tag:
                        var tagFunc = MarkDialogueTagInstruction.FromScriptLine(scriptLine);
                        EvaluateTagFunc(tagFunc, state);
                        break;
                }
            }

            if (state.VariableStore != null)
            {
                state.VariableStore.SetMarkDialogueScriptVisited(state.Script.AssetPath);
            }

            OnDialogueEnd();
        }

        private void EvaluateTagFunc(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;
            Debug.Log($"[{scriptLineNumber}] => Logic '{tagFunc.Tag}' with command '{tagFunc.Args}'.");

            var handled = MarkDialogueBuiltinCommands.TryHandleBuiltInCommand(tagFunc, state);
            if (handled)
            {
                return;
            }

            // TODO: Handle user func instead.
        }

        protected abstract void OnDialogueStart(MarkDialoguePlayerState state);

        protected abstract void OnDialogueEnd();

        protected abstract void OnDialogueLine(MarkDialogueCharacter character, MarkDialogueLine dialogueLine, WaitForDialogueContinuation continuation);

        protected abstract void OnQuoteText(MarkDialogueLine quoteLine);

        protected abstract void OnDialogueChoices(WaitForChoicesContinuation continuation);
    }
}
