#nullable enable

using NovaDawnStudios.MarkDialogue.Interfaces;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MarkDialogueInlineScriptException : Exception
    {
        public IMDRunnerState PlayerState { get; }

        public MarkDialogueInlineScriptException(IMDRunnerState state, string message)
            : base(state.CreateLoggingString(message))
        {
            PlayerState = state;
        }
    }
}