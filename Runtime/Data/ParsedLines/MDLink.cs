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
            tagInstruction.TargetScript = match.Groups["target"].Value.Trim();
            if (tagInstruction.TargetScript.Length == 0)
            {
                throw new System.InvalidOperationException($"Link target '{rawLine}' on line {lineNumber} contained no text!");
            }

            tagInstruction.DisplayName = match.Groups["display"].Value.Trim();

            if (string.IsNullOrWhiteSpace(tagInstruction.DisplayName))
            {
                var dispName = tagInstruction.TargetScript;
                if (dispName[0] == '#')
                {
                    dispName = dispName.Substring(1);
                }

                tagInstruction.DisplayName = dispName.Trim();
                
            }

            return tagInstruction;
        }
    }
}
