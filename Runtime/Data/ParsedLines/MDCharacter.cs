#nullable enable

using System;
using System.Linq;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    public class MDCharacter : MDRawScriptLine
    {
        [field: SerializeField] public string CharacterIdentifier { get; set; } = "<UNKNOWN>";
        [field: SerializeField] public string Alias { get; set; } = "";
        [field: SerializeField] public string[] Attributes { get; set; } = Array.Empty<string>();

        public MDCharacter(string rawLine, int lineNumber)
            : base(rawLine, lineNumber)
        {
        }

        public static MDCharacter FromScriptLine(string rawLine, int lineNumber)
        {
            var character = new MDCharacter(rawLine, lineNumber);

            var match = MDRegexCollection.characterRegex.Match(rawLine);
            character.CharacterIdentifier = match.Groups["ident"].Value;
            character.Alias = match.Groups["alias"].Value;
            character.Attributes = match.Groups["attribs"].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            return character;
        }
    }
}
