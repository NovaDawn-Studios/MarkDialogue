#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public static class MarkDialogueCommandMethodLexer
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

            public SingleCommandLexerState(string cmdString)
            {
                commandStr = cmdString;
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

        public static MarkDialogueCommand ParseSingleCommand(string commandStr)
        {
            var state = new SingleCommandLexerState(commandStr);

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
                        continue;
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
                return new MarkDialogueCommand(state.methodName.ToString(), state.args.ToArray());
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

        public class MarkDialogueCommand
        {
            public string MethodName { get; }
            public string[] Args { get; }

            public MarkDialogueCommand(string methodName, string[] args)
            {
                MethodName = methodName;
                Args = args;
            }
        }
    }
}