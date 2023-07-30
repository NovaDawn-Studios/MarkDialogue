#nullable enable

using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using System.Linq;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public static class MDBuiltinConditionalExpressions
    {
        internal static bool? TryHandleBuiltInCommandMethod(MDRunnerState state, string methodName, string[] args)
        {
            return methodName.ToLower() switch
            {
                "visited" => HandleMethodVisited(state, args),
                "notvisited" => HandleMethodNotVisited(state, args),
                "eq" or "equal" or "equals" => HandleEquality(state, methodName, args),
                "neq" or "notequal" or "notequals" => !HandleEquality(state, methodName, args),
                "gt" or "greaterthan" => NumComp(args) > 0,
                "gte" => NumComp(args) >= 0,
                "lt" or "lessthan" => NumComp(args) < 0,
                "lte" => NumComp(args) <= 0,
                _ => null,
            };
        }

        private static bool? HandleMethodVisited(MDRunnerState state, string[] args)
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

        private static bool? HandleMethodNotVisited(MDRunnerState state, string[] args)
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

        private static bool? HandleEquality(MDRunnerState state, string methodName, string[] args)
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

        private static int? NumComp(string[] args)
        {
            if (args.Length != 2)
            {
                throw new InvalidOperationException($"Expected number comparison to have exactly 2 arguments, but got the following values: '{string.Join(", ", args)}'");
            }

            return NumComp(args[0], args[1]);
        }

        private static int? NumComp(string var1, string var2)
        {
            if (int.TryParse(var2, out int var2Int))
            {
                if (int.TryParse(var1, out int var1Int))
                {
                    return var1Int.CompareTo(var2Int);
                }
                else if (double.TryParse(var1, out double var1Dbl))
                {
                    return var1Dbl.CompareTo(var2Int);
                }
            }
            else if (double.TryParse(var1, out double var1Dbl) && double.TryParse(var2, out double var2Dbl))
            {
                return var1Dbl.CompareTo(var2Dbl);
            }

            return var1.Trim().Equals(var2.Trim().ToString(), StringComparison.OrdinalIgnoreCase) ? 0 : null;
        }
    }
}