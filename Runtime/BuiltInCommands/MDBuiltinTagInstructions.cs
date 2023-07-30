#nullable enable

using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public static class MDBuiltinTagInstructions
    {
        /// <summary>
        ///     Runs any builtin tag commands. If any of these are successfully ran, then user-made commands will not be ran.
        /// </summary>
        /// <param name="tagFunc">The tag function line to run.</param>
        /// <param name="state">The current dialogue player state.</param>
        /// <returns><see langword="true"/> if the tag has been handled, <see langword="false"/> if user tags can handle it.</returns>
        /// <exception cref="MarkDialogueInlineScriptException">Thrown if the #throw tag is encountered.</exception>
        public static bool TryHandleBuiltInCommand(MDTagInstruction tagFunc, MDRunnerState state)
        {
            switch (tagFunc.Tag.ToLower())
            {
                case "end":
                    state.IsComplete = true;
                    return true;

                case "if":
                case "elseif":
                    return HandleIfBlock(tagFunc, state);

                case "else":
                    return HandleElseBlock(tagFunc, state);

                case "set":
                    if (state.VariableStore == null)
                    {
                        throw new MissingMarkDialogueVariableStoreException(state);
                    }
                    var spl = tagFunc.Args.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (spl.Length != 2)
                    {
                        // TODO: Use a proper exception type.
                        throw new InvalidOperationException($"Could not parse args for #set command '{tagFunc.Args}'");
                    }
                    state.VariableStore.SetMarkDialogueVariable(spl[0], spl[1]);
                    return true;

                case "todo":
                    Debug.LogWarning(state.CreateLoggingString("TODO tag encountered in script"));
                    return true;

                case "debug":
                    Debug.Log(state.CreateLoggingString(tagFunc.Args));
                    return true;

                case "warn":
                    Debug.LogWarning(state.CreateLoggingString(tagFunc.Args));
                    return true;

                case "error":
                    Debug.LogError(state.CreateLoggingString(tagFunc.Args));
                    return true;

                case "throw":
                    throw new MarkDialogueInlineScriptException(state, tagFunc.Args);

                default:
                    return false;
            }
        }

        private static bool HandleIfBlock(MDTagInstruction tagFunc, MDRunnerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;

            if (!state.EvaluateComparisonExpression(tagFunc.Args)) // Skip to the #else or #endif if the comparison failed.
            {           
                var ifEndTags = new[] { "else", "elseif", "endif" };
                var ifStartScopeTags = new[] { "if" };
                var ifEndScopeTags = new[] { "endif" };
                var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

                if (newTag == null)
                {
                    Debug.LogWarning(state.CreateLoggingString($"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?"));
                }
                if (newTag != null && newTag.Equals("elseif", StringComparison.OrdinalIgnoreCase))
                {
                    // Backup one line so this gets re-evaluated next line.
                    --state.CurrentScriptLineNumber;
                }
            }

            return true;
        }

        private static bool HandleElseBlock(MDTagInstruction tagFunc, MDRunnerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;

            var ifEndTags = new[] { "endif" };
            var ifStartScopeTags = new[] { "if" };
            var ifEndScopeTags = new[] { "endif" };
            var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

            if (newTag == null)
            {
                Debug.LogWarning(state.CreateLoggingString($"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?"));
            }

            return true;
        }
    }
}