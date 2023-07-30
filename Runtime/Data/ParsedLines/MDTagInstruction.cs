#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDTagInstruction : MDRawScriptLine
    {
        [field: SerializeField] public string Tag { get; set; } = "<UNKNOWN>";
        [field: SerializeField] public string Args { get; set; } = "";

        public MDTagInstruction(string rawLine, int lineNumber)
            : base(rawLine, lineNumber)
        {
        }

        public static MDTagInstruction FromScriptLine(string rawLine, int lineNumber)
        {
            var tagInstruction = new MDTagInstruction(rawLine, lineNumber);

            var match = MDRegexCollection.tagRegex.Match(rawLine);
            tagInstruction.Tag = match.Groups["tag"].Value;
            tagInstruction.Args = match.Groups["args"].Value;

            return tagInstruction;
        }
    }
}
