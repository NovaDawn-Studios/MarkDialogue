#nullable enable

using NovaDawnStudios.MarkDialogue.Interfaces;
using System;
using System.Collections.Generic;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDRunnerState : IMDRunnerState
    {
        /// <summary>The player that's running this dialogue.</summary>
        public MDBaseRunner Player { get; }

        /// <summary>
        ///     The variable store that existed when this script started running, if any. 
        ///     Don't forget to set this if your scripts run any logic!
        /// </summary>
        public IMDVariableStore? VariableStore { get; set; }

        /// <summary>The current collection that we're running a script from.</summary>
        public MDScriptCollectionAsset Collection { get; private set; }

        /// <summary>The current script we're running..</summary>
        public MDScriptAsset Script { get; private set; }

        /// <summary>The current line index in the current <see cref="Script"/>.</summary>
        public int CurrentScriptLineNumber { get; set; }

        /// <summary><see langword="true"/> if the dialogue is about to complete. If <see langword="true"/>, any unparsed dialogue will be skipped.</summary>
        public bool IsComplete { get; set; }

        /// <summary>The character who last spoke last.</summary>
        public MDCharacter? CurrentCharacter { get; set; }

        /// <summary>
        ///     <para>
        ///         The current list of available choices the player can choose when the script ends.
        ///     </para>
        ///     <para>
        ///         If this is empty, the script ends cleanly and <see cref="Player"/> ends execution. 
        ///     </para>
        ///     <para>
        ///         If there is only one entry in this when a script concludes, <see cref="Player"/> will jump to that script with no player intervention.
        ///     </para>
        /// </summary>
        public List<MDLink> Choices { get; set; } = new List<MDLink>();

        public MDRunnerState(MDBaseRunner player, MDScriptCollectionAsset initialCollection, MDScriptAsset initialScript)
        {
            Player = player;
            Collection = initialCollection;
            Script = initialScript;
            CurrentScriptLineNumber = 0;
        }

        /// <summary>
        ///     Creates a standardized logging string for MarkDialogue use. Includes the currently running script and collection, the line number we're
        ///     at and the supplied <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message we're about to log.</param>
        /// <returns>The supplied <paramref name="message"/> with extra formatting details.</returns>
        public string CreateLoggingString(string message)
        {
            return $"[{Collection.name}/{Script.name} @ {CurrentScriptLineNumber + Script.StartLine}] {message}";
        }

        /// <summary>
        ///     Starts a new script, optionally from a new collection. This resets a number of values and marks the previous script as having been visited.
        /// </summary>
        /// <param name="newScript">The new script we're launching.</param>
        /// <param name="newCollection"></param>
        public void StartNewScript(MDScriptAsset newScript, MDScriptCollectionAsset? newCollection = null)
        {
            if (VariableStore != null)
            {
                VariableStore.SetMarkDialogueScriptVisited(Script.AssetPath);
            }

            if (newCollection != null)
            {
                Collection = newCollection;
            }

            Script = newScript;
            CurrentScriptLineNumber = 0;
            Choices.Clear();
        }

        public string? SkipToNextTag(string[] targetTags, string[]? scopeStartTags = null, string[]? scopeEndTags = null)
        {
            int scopeLevel = 0;
            while (++CurrentScriptLineNumber < Script.Lines.Count)
            {
                var match = MDRegexCollection.tagRegex.Match(Script.Lines[CurrentScriptLineNumber].RawLine);
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

        public string ResolveScriptPath(string relativePath)
        {
            if (relativePath.Equals("this", StringComparison.OrdinalIgnoreCase))
            {
                return Script.AssetPath;
            }

            // TODO: Implement
            return relativePath;
        }

        public bool EvaluateComparisonExpression(string comparisonExpression)
        {
            var cmd = MDCommandMethodLexer.ParseCommandString(comparisonExpression);
            return InternalEvaluateComparisonExpressionRecursive(cmd, this);
        }

        private static bool InternalEvaluateComparisonExpressionRecursive(MDCommandMethodLexer.IMDCommand? cmd, MDRunnerState state)
        {
            if (cmd == null)
            {
                return false;
            }

            if (cmd is MDCommandMethodLexer.MDCommandGroup mdCommandGroup)
            {
                var res = InternalEvaluateComparisonExpressionRecursive(mdCommandGroup.Left, state);
                if (mdCommandGroup.ComparisonType == MDCommandMethodLexer.EMDCommandGroupComparisonType.And && !res)
                {
                    // AND failed. No need to try right. 
                    return false;
                }
                if (mdCommandGroup.ComparisonType == MDCommandMethodLexer.EMDCommandGroupComparisonType.Or && res)
                {
                    // OR succeeded. No need to try right. 
                    return true;
                }

                return InternalEvaluateComparisonExpressionRecursive(mdCommandGroup.Right, state);
            }

            if (cmd is MDCommandMethodLexer.MDCommand mdCommand)
            {
                return MDBuiltinConditionalExpressions.TryHandleBuiltInCommandMethod(state, mdCommand.MethodName, mdCommand.Args) 
                    ?? state.Player.EvaluateComparisonFunction(mdCommand.MethodName, mdCommand.Args, state);
            }

            throw new NotImplementedException($"Unknown command type {cmd.GetType()}");
        }
    }
}
