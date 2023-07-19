#nullable enable

using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MarkDialogueScriptArgumentsException : Exception
    {
        public MarkDialoguePlayerState PlayerState { get; }

        public MarkDialogueScriptArgumentsException(MarkDialoguePlayerState state, string message)
            : base(state.CreateLoggingString(message))
        {
            PlayerState = state;
        }
    }
}