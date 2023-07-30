#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDQuoteLine : MDRawScriptLine
    {
        [field: SerializeField] public string LineText { get; set; } = "<UNKNOWN>";

        public MDQuoteLine(string rawLine, int lineNumber)
            : base(rawLine, lineNumber)
        {
        }

        public static MDQuoteLine FromScriptLine(string rawLine, int lineNumber)
        {
            var mdLine = new MDQuoteLine(rawLine, lineNumber);
            mdLine.LineText = rawLine;
            return mdLine;
        }
    }
}
