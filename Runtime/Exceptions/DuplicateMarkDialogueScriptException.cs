#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class DuplicateMarkDialogueScriptException : Exception
    {
        public MDScriptCollectionAsset ScriptCollection { get; }
        public MDScriptAsset DuplicatedScript { get; }

        public DuplicateMarkDialogueScriptException(MDScriptCollectionAsset collection, MDScriptAsset script)
            : base($"A duplicate script with name '{script.name}' was encountered while parsing collection '{collection.name}'. Please change the name of one of the scripts and re-import.")
        {
            ScriptCollection = collection;
            DuplicatedScript = script;
        }
    }
}