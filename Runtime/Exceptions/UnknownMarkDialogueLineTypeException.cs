#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;

namespace NovaDawnStudios.MarkDialogue.Exceptions
{
    public class UnknownMarkDialogueLineTypeException : Exception
    {
        public MDRawScriptLine ScriptLine { get; }

        public UnknownMarkDialogueLineTypeException(MDRawScriptLine scriptLine)
            : base($"Received a script line with a line type of '{scriptLine.GetType()}' which is unknown to MarkDialogue.")
        {
            ScriptLine = scriptLine;
        }
    }
}