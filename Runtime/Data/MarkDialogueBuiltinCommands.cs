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
        /// <summary>
        ///     Runs any builtin tag commands. If any of these are successfully ran, then user-made commands will not be ran.
        /// </summary>
        /// <param name="tagFunc">The tag function line to run.</param>
        /// <param name="state">The current dialogue player state.</param>
        /// <returns><see langword="true"/> if the tag has been handled, <see langword="false"/> if user tags can handle it.</returns>
        /// <exception cref="MarkDialogueInlineScriptException">Thrown if the #throw tag is encountered.</exception>
        public static bool TryHandleBuiltInCommand(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
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

        private static bool? TryHandleBuiltInCommandMethod(MarkDialoguePlayerState state, string methodName, string[] args)
        {
            switch (methodName.ToLower())
            {
                case "visited":
                    return HandleMethodVisited(state, args);

                case "notvisited":
                    return HandleMethodNotVisited(state, args);

                case "eq":
                case "equal":
                case "equals":
                    return HandleEquality(state, methodName, args);

                case "neq":
                case "notequal":
                case "notequals":
                    return !HandleEquality(state, methodName, args);

                case "gt":
                case "greaterthan":
                    // TODO
                    throw new NotImplementedException();

                case "gte":
                    // TODO
                    throw new NotImplementedException();

                case "lt":
                case "lessthan":
                    // TODO
                    throw new NotImplementedException();

                case "lte":
                    // TODO
                    throw new NotImplementedException();
            }

            return null;
        }

        private static bool HandleIfBlock(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
        {
            int scriptLineNumber = state.CurrentScriptLineNumber;

            // TODO: Handle multiple commands
            var cmd = MarkDialogueCommandMethodLexer.ParseSingleCommand(tagFunc.Args);
            var cmdResult = TryHandleBuiltInCommandMethod(state, cmd.MethodName, cmd.Args);

            if (cmdResult == null)
            {
                // TODO: Get the dialogue player to handle a possible custom command. For now, warn and assume false.
                Debug.LogWarning($"Unhandled command '{cmd.MethodName}' with arguments {string.Join(", ", cmd.Args)}");
                cmdResult = false;
            }

            if (cmdResult == false) // Skip to the #else or #endif if the comparison failed.
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

        private static bool HandleElseBlock(MarkDialogueTagInstruction tagFunc, MarkDialoguePlayerState state)
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

        #region Built in command methods

        private static bool? HandleMethodVisited(MarkDialoguePlayerState state, string[] args)
        {
            if (args.Length == 0)
            {
                throw new MarkDialogueScriptArgumentsException(state, $"visited() expects at least 1 argument");
            }

            int minCount = 1;
            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out int parsedMinCount))
                {
                    throw new MarkDialogueScriptArgumentsException(state, $"Expected second argument for visited() to be an integer. Got '{args[1]}' instead.");
                }

                minCount = parsedMinCount;
            }

            if (minCount < 1)
            {
                throw new MarkDialogueScriptArgumentsException(state, $"visited() expects a visit count to be 1 or greater. Did you mean to use notvisited()?");
            }

            if (state.VariableStore == null)
            {
                throw new MissingMarkDialogueVariableStoreException(state);
            }

            var absoluteScriptPath = state.ResolveScriptPath(args[0]);
            return state.VariableStore.GetMarkDialogueScriptVisitCount(absoluteScriptPath) >= minCount;
        }

        private static bool? HandleMethodNotVisited(MarkDialoguePlayerState state, string[] args)
        {
            if (args.Length == 0)
            {
                throw new MarkDialogueScriptArgumentsException(state, $"visited() expects at least 1 argument");
            }

            if (state.VariableStore == null)
            {
                throw new MissingMarkDialogueVariableStoreException(state);
            }

            var absoluteScriptPath = state.ResolveScriptPath(args[0]);
            return state.VariableStore.GetMarkDialogueScriptVisitCount(absoluteScriptPath) == 0;
        }

        private static bool? HandleEquality(MarkDialoguePlayerState state, string methodName, string[] args)
        {
            if (args.Length != 2)
            {
                throw new MarkDialogueScriptArgumentsException(state, $"{methodName}() expects exactly 2 arguments");
            }

            if (state.VariableStore == null)
            {
                throw new MissingMarkDialogueVariableStoreException(state);
            }

            var var1 = state.VariableStore.GetMarkDialogueVariable(args[0]);

            if (var1 == null)
            {
                return false;
            }

            if (args[1].StartsWith('"') && args[1].EndsWith('"'))
            {
                return var1.Equals(args[1].Substring(1, args[1].Length - 2));
            }

            if (double.TryParse(args[1], out _))
            {
                var numCompConstRes = NumComp(var1, args[1]);
                if (numCompConstRes.HasValue)
                {
                    return numCompConstRes.Value == 0;
                }

                return false;
            }

            var var2 = state.VariableStore.GetMarkDialogueVariable(args[0]);
            if (var2 == null)
            {
                return false;
            }

            var numCompRes = NumComp(var1, var2);
            return numCompRes.HasValue && numCompRes.Value == 0;
        }

        private static int? NumComp(string var1, string var2)
        {
            if (int.TryParse(var2, out int intNum))
            {
                if (int.TryParse(var1, out int intVarNum))
                {
                    return intVarNum.CompareTo(intNum);
                }
                else if (double.TryParse(var1, out double dblVarNum))
                {
                    return dblVarNum.CompareTo(intNum);
                }
            }
            else if (double.TryParse(var1, out double dblVarNum) && double.TryParse(var2, out double doubleNum))
            {
                return dblVarNum.CompareTo(doubleNum);
            }

            return var1.Trim().Equals(var2.Trim().ToString(), StringComparison.OrdinalIgnoreCase) ? 0 : null;
        }

        #endregion
    }
}