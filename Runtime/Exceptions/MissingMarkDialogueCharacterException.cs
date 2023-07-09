#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MissingMarkDialogueCharacterException : Exception
    {
        public MarkDialogueScriptCollection ScriptCollection { get; }
        public MarkDialogueScript Script { get; }
        public MarkDialogueScriptLine Line { get; }

        public MissingMarkDialogueCharacterException(MarkDialogueScriptCollection collection, MarkDialogueScript script, MarkDialogueScriptLine line)
            : base($"Tried to output a dialogue line but no character was set. Error happened at line {line.lineNumber} in script collection {collection.name} - script {script.name}.")
        {
            ScriptCollection = collection;
            Script = script;
            Line = line;
        }
    }
}