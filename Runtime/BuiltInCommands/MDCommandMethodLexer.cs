#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Experimental.AI;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public static class MDCommandMethodLexer
    {
        private class SingleCommandLexerState
        {
            public string commandStr;
            public int i;
            public StringBuilder methodName = new();
            public StringBuilder currentArg = new();
            public List<string> args = new();
            public bool hasCompletedMethodName = false;
            public bool isInArgs = false;
            public bool isInQuotes = false;
            public bool hasUsedQuotedString = false;
            public bool isComplete = false;

            public SingleCommandLexerState(string cmdString, int startIndex)
            {
                commandStr = cmdString;
                i = startIndex;
            }

            public void AddCurrentArg()
            {
                var arg = currentArg.ToString();
                if (!hasUsedQuotedString)
                {
                    arg = arg.Trim();
                }
                args.Add(arg);
                currentArg = new StringBuilder();
                hasUsedQuotedString = false;
            }
        }

        public static IMDCommand ParseCommandString(string commandStr)
        {
            var (cmd, _) = ParseCommandStringInternal(commandStr, 0);
            return cmd;
        }

        private static (IMDCommand?, int) ParseCommandStringInternal(string commandStr, int startIndex)
        {
            var group = new MDCommandGroup();

            for (int i = startIndex; i < commandStr.Length; ++i)
            {
                var chr = commandStr[i];
                if (char.IsWhiteSpace(chr))
                {
                    continue;
                }

                if (chr == '(')
                {
                    var (cmd, idx) = ParseCommandStringInternal(commandStr, i + 1);
                    i = idx;
                    if (group.AddCommand(cmd))
                    {
                        return (group, i);
                    }
                    continue;
                }
                else if (chr == ')' && startIndex > 0)
                {
                    if (group.Right != null)
                    {
                        return (group, i);
                    }
                    else
                    {
                        return (group.Left, i);
                    }
                }

                if (i < commandStr.Length - 4 && commandStr.Substring(i, 4).Equals("and ", StringComparison.OrdinalIgnoreCase))
                {
                    group.ComparisonType = EMDCommandGroupComparisonType.And;
                    var (cmd, idx) = ParseCommandStringInternal(commandStr, i + 4);
                    i = idx;
                    if (group.AddCommand(cmd))
                    {
                        return (group, i);
                    }
                }
                else if (i < commandStr.Length - 3 && commandStr.Substring(i, 3).Equals("or ", StringComparison.OrdinalIgnoreCase))
                {
                    group.ComparisonType = EMDCommandGroupComparisonType.Or;
                    var (cmd, idx) = ParseCommandStringInternal(commandStr, i + 3);
                    i = idx;
                    if (group.AddCommand(cmd))
                    {
                        return (group, i);
                    }
                }
                else
                {
                    var (cmd, idx) = ParseSingleCommand(commandStr, i);
                    i = idx;
                    if (group.AddCommand(cmd))
                    {
                        return (group, i);
                    }
                }
            }

            if (group.Right != null)
            {
                return (group, commandStr.Length);
            }
            else
            {
                return (group.Left, commandStr.Length);
            }
        }


        public static (MDCommand, int) ParseSingleCommand(string commandStr, int startIndex = 0)
        {
            var state = new SingleCommandLexerState(commandStr, startIndex);

            for (; !state.isComplete && state.i < commandStr.Length; ++state.i)
            {
                if (!state.isInArgs)
                {
                    ParseMethodName(state);
                }
                else
                {
                    var chr = state.commandStr[state.i];
                    if (state.isInQuotes)
                    {
                        ParseQuotedStringArgument(state);
                        continue;
                    }
                    else if (chr == '"' && state.currentArg.Length == 0)
                    {
                        state.isInQuotes = true;
                        continue;
                    }
                    else if (chr == ')')
                    {
                        if (state.currentArg.Length > 0)
                        {
                            state.AddCurrentArg();
                        }

                        state.isComplete = true;
                        break;
                    }
                    else if (chr == ',')
                    {
                        state.AddCurrentArg();
                        continue;
                    }

                    if (char.IsWhiteSpace(chr))
                    {
                        if (state.currentArg.Length == 0 || state.hasUsedQuotedString)
                        {
                            continue;
                        }
                    }
                    else if (state.hasUsedQuotedString)
                    {
                        throw new InvalidOperationException($"Failed to parse command string '{commandStr}' - Encountered a character after parsing quoted argument '{state.currentArg}'.");
                    }

                    state.currentArg.Append(chr);
                }
            }

            if (state.isComplete)
            {
                return (new MDCommand(state.methodName.ToString(), state.args.ToArray()), state.i);
            }

            throw new InvalidOperationException($"Failed to parse command string '{state.commandStr}' - Either the string is lacking a closing bracket, or the command string is malformed in an unparsable fashion.");
        }

        private static void ParseMethodName(SingleCommandLexerState state)
        {
            var chr = state.commandStr[state.i];
            if (char.IsWhiteSpace(chr))
            {
                if (state.methodName.Length > 0)
                {
                    state.hasCompletedMethodName = true;
                }
                return;
            }

            if (chr == '(')
            {
                if (state.methodName.Length == 0)
                {
                    throw new InvalidOperationException($"Failed to parse command string '{state.commandStr}' - No method name could be determined before opening bracket.");
                }

                state.hasCompletedMethodName = true;
                state.isInArgs = true;
            }
            else if (state.hasCompletedMethodName)
            {
                throw new InvalidOperationException($"Failed to parse command string '{state.commandStr}' - Method name contains whitespace.");
            }
            else
            {
                state.methodName.Append(chr);
            }
        }

        private static void ParseQuotedStringArgument(SingleCommandLexerState state)
        {
            var chr = state.commandStr[state.i];
            if (chr == '\\' && state.commandStr[state.i + 1] == '"')
            {
                return;
            }
            if (chr == '"' && state.commandStr[state.i - 1] != '\\')
            {
                state.isInQuotes = false;
                state.hasUsedQuotedString = true;
                return;
            }

            state.currentArg.Append(chr);
        }

        public interface IMDCommand { }

        public enum EMDCommandGroupComparisonType
        {
            And,
            Or
        }

        public class MDCommandGroup : IMDCommand
        {
            public IMDCommand? Left { get; set; }
            public IMDCommand? Right { get; set; }
            public EMDCommandGroupComparisonType ComparisonType { get; set; } = EMDCommandGroupComparisonType.And;

            public bool AddCommand(IMDCommand cmd)
            {
                if (Left == null)
                {
                    Left = cmd;
                    return false;
                }
                else if (Right == null)
                {
                    Right = cmd;
                    return true;
                }

                throw new InvalidOperationException($"Tried to add a third command to an already full command group. This should not happen!");
            }
        }

        public class MDCommand : IMDCommand
        {
            public string MethodName { get; }
            public string[] Args { get; }

            public MDCommand(string methodName, string[] args)
            {
                MethodName = methodName;
                Args = args;
            }
        }
    }
}