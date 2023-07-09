#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class DuplicateMarkDialogueScriptException : Exception
    {
        public MarkDialogueScriptCollection ScriptCollection { get; }
        public MarkDialogueScript DuplicatedScript { get; }

        public DuplicateMarkDialogueScriptException(MarkDialogueScriptCollection collection, MarkDialogueScript script)
            : base($"A duplicate script with name '{script.name}' was encountered while parsing collection '{collection.name}'. Please change the name of one of the scripts and re-import.")
        {
            ScriptCollection = collection;
            DuplicatedScript = script;
        }
    }
}