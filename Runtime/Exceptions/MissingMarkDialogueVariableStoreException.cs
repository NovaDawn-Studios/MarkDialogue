#nullable enable

using NovaDawnStudios.MarkDialogue.Interfaces;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MissingMarkDialogueVariableStoreException : Exception
    {
        public IMDRunnerState State { get; }

        public MissingMarkDialogueVariableStoreException(IMDRunnerState state)
            : base(state.CreateLoggingString($"No {nameof(state.VariableStore)} has been set! Make sure you create an instance and apply it to the state object's {nameof(state.VariableStore)} property in your MarkDialogue player's 'OnDialogueStart' method!"))
        {
            State = state;
        }
    }
}