#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MarkDialogueInlineScriptException : Exception
    {
        public MarkDialoguePlayerState PlayerState { get; }

        public MarkDialogueInlineScriptException(MarkDialoguePlayerState state, string message)
            : base(MarkDialogueBuiltinCommands.CreateLoggingString(state, message))
        {
            PlayerState = state;
        }
    }
}