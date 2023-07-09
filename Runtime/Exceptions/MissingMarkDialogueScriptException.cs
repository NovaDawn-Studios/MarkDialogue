#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MissingMarkDialogueScriptException : Exception
    {
        public MarkDialogueScriptCollection ScriptCollection { get; }
        public string ExpectedScriptName { get; }

        public MissingMarkDialogueScriptException(MarkDialogueScriptCollection collection, string expectedScriptName)
            : base($"Could not find script of name {expectedScriptName} for collection {collection.name}.")
        {
            ScriptCollection = collection;
            ExpectedScriptName = expectedScriptName;
        }
    }
}