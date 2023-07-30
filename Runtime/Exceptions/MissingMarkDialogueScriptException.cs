#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class MissingMarkDialogueScriptException : Exception
    {
        public MDScriptCollectionAsset? ScriptCollection { get; }
        public string ExpectedScriptName { get; }
        public string ScriptPath { get; }

        public MissingMarkDialogueScriptException(MDScriptCollectionAsset collection, string expectedScriptName)
            : base($"Could not find script of name '{expectedScriptName}' for collection '{collection.name}'.")
        {
            ScriptCollection = collection;
            ExpectedScriptName = expectedScriptName;
            ScriptPath = $"{collection.AssetPath}#{expectedScriptName}";
        }

        public MissingMarkDialogueScriptException(string scriptPath)
            : base($"Could not find script at path '{scriptPath}'.")
        {
            ScriptPath = scriptPath;
            if (scriptPath.Contains('#'))
            {
                ExpectedScriptName = scriptPath.Substring(scriptPath.IndexOf('#') + 1);
            }
            else
            {
                ExpectedScriptName = MDScriptAsset.DEFAULT_SCRIPT_NAME;
            }
        }
    }
}