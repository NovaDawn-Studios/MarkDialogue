#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     Provides the core functionality for running a MarkDialogue script. 
    ///     Inherit from this class to provide your own implementation as require by your project. 
    /// </summary>
    public abstract class MDBaseRunner : MonoBehaviour
    {
        /// <summary>The script collection containing the MarkDialogue script we want to run.</summary>
        [field: SerializeField] public MDScriptCollectionAsset? ScriptCollection { get; set; }

        /// <summary>
        ///     The name of the MarkDialogue script to run. 
        ///     This defaults to the default script. Not used if <see cref="ScriptCollection"/> only contains one script.
        /// </summary>
        [field: SerializeField] public string ScriptToRun { get; set; } = MDScriptAsset.DEFAULT_SCRIPT_NAME;

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

        private IEnumerator DialoguePlaybackLoop(MDScriptCollectionAsset collection, MDScriptAsset script)
        {
            var state = new MDRunnerState(this, collection, script);

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
                        var continuation = new WaitForMarkDialogueChoices(state.Choices);
                        OnDialogueChoices(continuation);
                        yield return continuation;
                        selectedChoice = continuation.SelectedChoice;

                        if (selectedChoice == null)
                        {
                            throw new InvalidOperationException($"Expected {nameof(WaitForMarkDialogueChoices)} to return a choice, but got null instead!");
                        }
                    }

                    var choicePath = state.Collection.ResolveRelativePathTo(selectedChoice.TargetScript);
                    var link = state.Script.LinkedToScripts.Find(s => s.AssetPath.Equals(choicePath, StringComparison.OrdinalIgnoreCase));
                    if (link == null)
                    {
                        throw new MissingMarkDialogueScriptException(choicePath);
                    }

                    state.StartNewScript(link);
                }

                var scriptLine = state.Script.Lines[state.CurrentScriptLineNumber];

                switch (scriptLine)
                {
                    case MDCharacter charLine:
                        state.CurrentCharacter = charLine;
                        break;

                    case MDDialogueLine dialogueLine:
                        if (state.CurrentCharacter == null)
                        {
                            throw new MissingMarkDialogueCharacterException(collection, script, scriptLine);
                        }

                        var continuator = new WaitForDialogueResumption();
                        OnDialogueLine(state.CurrentCharacter, dialogueLine, continuator);
                        yield return continuator;
                        break;

                    case MDLink linkLine:
                        state.Choices.Add(linkLine);
                        break;

                    case MDQuoteLine quoteLine:
                        quoteLine.LineText = quoteLine.LineText[1..].Trim(); // Remove the leading '>' character.
                        OnQuoteText(quoteLine);
                        break;

                    case MDTagInstruction tagInstruction:
                        EvaluateTagFunc(tagInstruction, state);
                        break;

                    default:
                        throw new UnknownMarkDialogueLineTypeException(scriptLine);
                }
            }

            if (state.VariableStore != null)
            {
                state.VariableStore.SetMarkDialogueScriptVisited(state.Script.AssetPath);
            }

            OnDialogueEnd();
        }

        private void EvaluateTagFunc(MDTagInstruction tagInstruction, MDRunnerState state)
        {
            var handled = MDBuiltinTagInstructions.TryHandleBuiltInCommand(tagInstruction, state);
            if (!handled)
            {
                HandleTagInstructions(tagInstruction, state);
            }
        }

        protected abstract void OnDialogueStart(MDRunnerState state);

        protected abstract void OnDialogueEnd();

        protected abstract void OnDialogueLine(MDCharacter character, MDDialogueLine dialogueLine, WaitForDialogueResumption continuation);

        protected abstract void OnQuoteText(MDQuoteLine quoteLine);

        protected abstract void OnDialogueChoices(WaitForMarkDialogueChoices continuation);

        protected internal virtual void HandleTagInstructions(MDTagInstruction tagInstruction, MDRunnerState state) { }
        protected internal virtual bool EvaluateComparisonFunction(string methodName, string[] methodArguments, MDRunnerState state) 
        {
            Debug.LogWarning($"Unahndle MarkDialogue Comparison function");
            return false;
        }
    }
}
