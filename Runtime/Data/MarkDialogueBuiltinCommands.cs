#nullable enable

using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public static class MarkDialogueBuiltinCommands
    {
        public static string CreateLoggingString(MarkDialoguePlayerState state, string message)
        {
            string collectionName = "<unknown collection>";
            string scriptName = "<unknown script>";
            if (state.Collection != null)
            {
                collectionName = state.Collection.name;
            }
            if (state.Script != null)
            {
                scriptName = state.Script.name;
            }

            return $"[{collectionName}/{scriptName} @ {state.CurrentScriptLineNumber}] {message}";
        }

        /// <summary>
        ///     Runs any builtin tag commands. If any of these are successfully ran, then user-made commands will not be ran.
        /// </summary>
        /// <param name="tagFunc">The tag function line to run.</param>
        /// <param name="state">The current dialogue player state.</param>
        /// <returns><see langword="true"/> if the tag has been handled, <see langword="false"/> if user tags can handle it.</returns>
        /// <exception cref="MarkDialogueInlineScriptException">Thrown if the #throw tag is encountered.</exception>
        public static bool RunBuiltinCommand(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            switch (tagFunc.Tag.ToLower())
            {
                case "if":
                case "elseif":
                    return HandleIfBlock(tagFunc, state);
                case "else":
                    return HandleElseBlock(tagFunc, state);
                case "todo":
                    Debug.LogWarning(CreateLoggingString(state, "TODO tag encountered in script"));
                    return true;
                case "debug":
                    Debug.Log(CreateLoggingString(state, tagFunc.Args));
                    return true;
                case "warn":
                    Debug.LogWarning(CreateLoggingString(state, tagFunc.Args));
                    return true;
                case "error":
                    Debug.LogError(CreateLoggingString(state, tagFunc.Args));
                    return true;
                case "throw":
                    throw new MarkDialogueInlineScriptException(state, tagFunc.Args);
                default:
                    return false;
            }
        }

        private static bool HandleIfBlock(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;

            // TODO: Parse functions
            if (UnityEngine.Random.value < 0.5)
            {
                var ifEndTags = new[] { "else", "elseif", "endif" };
                var ifStartScopeTags = new[] { "if" };
                var ifEndScopeTags = new[] { "endif" };
                var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

                if (newTag == null)
                {
                    Debug.LogWarning(CreateLoggingString(state, $"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?"));
                }
                if (newTag != null && newTag.Equals("elseif", StringComparison.OrdinalIgnoreCase))
                {
                    // Backup one line so this gets re-evaluated next line.
                    --state.CurrentScriptLineNumber;
                }
            }

            return true;
        }

        private static bool HandleElseBlock(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;

            var ifEndTags = new[] { "endif" };
            var ifStartScopeTags = new[] { "if" };
            var ifEndScopeTags = new[] { "endif" };
            var newTag = state.SkipToNextTag(ifEndTags, ifStartScopeTags, ifEndScopeTags);

            if (newTag == null)
            {
                Debug.LogWarning(CreateLoggingString(state, $"Unterminated '{tagFunc.Tag}' tag on line {scriptLineNumber}. Did you forget an #endif?"));
            }

            return true;
        }
    }
}