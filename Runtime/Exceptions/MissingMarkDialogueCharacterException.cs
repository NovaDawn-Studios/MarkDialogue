#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MissingMarkDialogueCharacterException : Exception
    {
        public MDScriptCollectionAsset ScriptCollection { get; }
        public MDScriptAsset Script { get; }
        public MDRawScriptLine Line { get; }

        public MissingMarkDialogueCharacterException(MDScriptCollectionAsset collection, MDScriptAsset script, MDRawScriptLine line)
            : base($"Tried to output a dialogue line but no character was set. Error happened at line {line.LineNumber} in script collection {collection.name} - script {script.name}.")
        {
            ScriptCollection = collection;
            Script = script;
            Line = line;
        }
    }
}