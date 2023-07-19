#nullable enable

using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MarkDialogueInlineScriptException : Exception
    {
        public MarkDialoguePlayerState PlayerState { get; }

        public MarkDialogueInlineScriptException(MarkDialoguePlayerState state, string message)
            : base(state.CreateLoggingString(message))
        {
            PlayerState = state;
        }
    }
}