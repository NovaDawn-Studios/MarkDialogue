#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
            var state = new MarkDialoguePlayerState()
            {
                Collection = collection,
                Script = script,
                CurrentScriptLineNumber = script.StartLine,
            };

            OnDialogueStart(state);

            for(; state.CurrentScriptLineNumber <= state.Script.Lines.Count; ++state.CurrentScriptLineNumber)
            {
                if (state.CurrentScriptLineNumber == state.Script.Lines.Count)
                {
                    if (state.Choices.Count == 0)
                    {
                        // Nowhere to go. Dialogue ends.
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

                    // TODO: Parse selectedChoice
                    Debug.Log($"TODO: Jump to {selectedChoice.TargetScript} (Display name: {selectedChoice.DisplayName}");
                    break;
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

                yield return null;
            }

            OnDialogueEnd();
        }

        private void EvaluateTagFunc(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;
            Debug.Log($"[{scriptLineNumber}] => Logic '{tagFunc.Tag}' with command '{tagFunc.Args}'.");

            // TODO: Handle builtins using database class.
            if (tagFunc.Tag == "if" || tagFunc.Tag == "elseif")
            {
                // TODO: Parse functions
                if (UnityEngine.Random.value < 0.5)
                {
                    // Continue as is
                    return;
                }
                else
                {
                    var ifEndTags = new[] { "else", "elseif", "endif" };
                    var ifStartScopeTags = new[] { "if" };
                    var ifEndScopeTags = new[] { "endif" };
                    var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

                    if (newTag == null)
                    {
                        Debug.LogWarning($"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?");
                    }
                    if (newTag != null && newTag.Equals("elseif", StringComparison.OrdinalIgnoreCase))
                    {
                        // Backup one line so this gets re-evaluated next line.
                        --state.CurrentScriptLineNumber;
                    }
                }
            }
            else if (tagFunc.Tag == "else")
            {
                var ifEndTags = new[] { "endif" };
                var ifStartScopeTags = new[] { "if" };
                var ifEndScopeTags = new[] { "endif" };
                var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

                if (newTag == null)
                {
                    Debug.LogWarning($"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?");
                }
            }
        }

        protected abstract void OnDialogueStart(MarkDialoguePlayerState state);

        protected abstract void OnDialogueEnd();

        protected abstract void OnDialogueLine(MarkDialogueCharacter character, MarkDialogueLine dialogueLine, WaitForDialogueContinuation continuation);

        protected abstract void OnQuoteText(MarkDialogueLine quoteLine);

        protected abstract void OnDialogueChoices(WaitForChoicesContinuation continuation);

    }

    public class MarkDialoguePlayerState
    {
        public MarkDialogueScriptCollection? Collection { get; set; }
        public MarkDialogueScript? Script { get; set; }
        public int CurrentScriptLineNumber { get; set; }
        public MarkDialogueCharacter? CurrentCharacter { get; set; }
        public List<MarkDialogueLink> Choices { get; set; } = new List<MarkDialogueLink>();

        public string? SkipToNextTag(string[] targetTags, string[]? scopeStartTags = null, string[]? scopeEndTags = null)
        {
            if (Script == null)
            {
                throw new InvalidOperationException($"{nameof(Script)} cannot be null.");
            }

            int scopeLevel = 0;
            while (++CurrentScriptLineNumber < Script.Lines.Count)
            {
                var match = MarkDialogueRegexCollection.tagRegex.Match(Script.Lines[CurrentScriptLineNumber].rawLine);
                if (!match.Success)
                {
                    continue;
                }

                var tag = match.Groups["tag"].Value;

                if (scopeLevel == 0 && Array.Exists(targetTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    return tag;
                }

                if (scopeStartTags != null && Array.Exists(scopeStartTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    ++scopeLevel;
                }

                if (scopeEndTags != null && Array.Exists(scopeEndTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    --scopeLevel;
                }
            }

            return null;
        }
    }
}
