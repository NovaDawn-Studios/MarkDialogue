#nullable enable

using NovaDawnStudios.MarkDialogue.Interfaces;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MarkDialogueScriptArgumentsException : Exception
    {
        public IMDRunnerState PlayerState { get; }

        public MarkDialogueScriptArgumentsException(IMDRunnerState state, string message)
            : base(state.CreateLoggingString(message))
        {
            PlayerState = state;
        }
    }
}