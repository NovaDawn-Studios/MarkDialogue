#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDLink : MDRawScriptLine
    {
        [field: SerializeField] public string TargetScript { get; set; } = "";
        [field: SerializeField] public string DisplayName { get; set; } = "";

        public MDLink(string rawLine, int lineNumber)
            : base(rawLine, lineNumber)
        {
        }

        public static MDLink FromScriptLine(string rawLine, int lineNumber)
        {
            var tagInstruction = new MDLink(rawLine, lineNumber);

            var match = MDRegexCollection.linkRegex.Match(rawLine);
            tagInstruction.TargetScript = match.Groups["target"].Value;
            tagInstruction.DisplayName = match.Groups["display"].Value;
            if (string.IsNullOrWhiteSpace(tagInstruction.DisplayName))
            {
                tagInstruction.DisplayName = tagInstruction.TargetScript;
            }

            return tagInstruction;
        }
    }
}
