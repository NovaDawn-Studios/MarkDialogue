#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDDialogueLine : MDRawScriptLine
    {
        [field: SerializeField] public string LineText { get; set; } = "<UNKNOWN>";

        public MDDialogueLine(string rawLine, int lineNumber)
            : base(rawLine, lineNumber)
        {
        }

        public static MDDialogueLine FromScriptLine(string rawLine, int lineNumber)
        {
            var mdLine = new MDDialogueLine(rawLine, lineNumber);
            mdLine.LineText = rawLine;
            return mdLine;
        }
    }
}
