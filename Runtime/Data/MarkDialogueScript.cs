#nullable enable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public sealed class MarkDialogueScript : ScriptableObject
    {
        internal const string DEFAULT_SCRIPT_NAME = "Default Script";

        /// <summary>
        ///     Matches on a MarkDown heading, capturing the text of the heading and stopping before an Obsidian comment or the end of the line.
        ///     This signifies the line represents the start of a new script within the file.
        /// </summary>
        private static readonly Regex headingRegex = new(@"^#{1,6}\s+(.*?)(?=%%|$)", RegexOptions.Compiled);

        /// <summary>
        ///     Matches one or more Unicode uppercase characters, hyphens and underscores at the start of the line.
        ///     This signifies the line represents a character.
        /// </summary>
        private static readonly Regex characterRegex = new(@"^[\p{Lu}-_]+(?=\s|$)", RegexOptions.Compiled);

        /// <summary>
        ///     Matches a link in the form <c>[[Some test]]</c> that either exists on its own line or ends with an Obsidian comment.
        /// </summary>
        private static readonly Regex linkRegex = new(@"^\[\[(.+)\]\](?=\s*%%|$)", RegexOptions.Compiled);

        [field: SerializeField] public string ScriptName { get; set; } = DEFAULT_SCRIPT_NAME;
        [field: SerializeField] public List<MarkDialogueScriptLine> Lines { get; set; } = new();

        /// <summary>
        ///     Parses the supplied pre-split script lines starting from <paramref name="lineNumber"/>. Returns the last line we get to before ending, which is either
        ///     the last line in the file, or the heading of the next script in a multi-script file.
        /// </summary>
        /// <param name="splScript">The lines to parse.</param>
        /// <param name="lineNumber">The starting line</param>
        /// <returns>The last line that we parsed to - either the end of the file, or the heading that starts a new script.</returns>
        public int Parse(string[] splScript, int lineNumber)
        {
            // Check if we have a section name
            string line = splScript[lineNumber].Trim();
            var match = headingRegex.Match(line);
            if (match.Success)
            {
                ScriptName = match.Groups[1].Value;
                ++lineNumber;
            }

            this.name = ScriptName;

            for (; lineNumber < splScript.Length; ++lineNumber)
            {
                line = splScript[lineNumber].Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (headingRegex.IsMatch(line)) // We've hit a new heading. Bounce out and let the next MarkDialogueScript handle it.
                {
                    break;
                }

                if (linkRegex.IsMatch(line))
                {
                    Lines.Add(new MarkDialogueScriptLine
                    {
                        type = MarkDialogueScriptLineType.Link,
                        rawLine = line,
                    });
                    continue;
                }

                if (line[0] == '#') // A safe check. The above heading Regex would have caught '# Anything', so this MUST be '#Anything' with no whitespace
                {
                    Lines.Add(new MarkDialogueScriptLine
                    {
                        type = MarkDialogueScriptLineType.Tag,
                        rawLine = line,
                    });
                    continue;
                }

                if (line[0] == '>')
                {
                    Lines.Add(new MarkDialogueScriptLine
                    {
                        type = MarkDialogueScriptLineType.Quote,
                        rawLine = line,
                    });
                    continue;
                }
                
                match = characterRegex.Match(line);
                if (match.Success)
                {
                    Lines.Add(new MarkDialogueScriptLine
                    {
                        type = MarkDialogueScriptLineType.Character,
                        rawLine = line,
                    });
                    continue;
                }

                // Must be a dialogue line, or something currently unhandled. Presume the best case scenario.
                Lines.Add(new MarkDialogueScriptLine
                {
                    type = MarkDialogueScriptLineType.Dialogue,
                    rawLine = line,
                });
            }

            return lineNumber;
        }
    }

    [Serializable]
    public enum MarkDialogueScriptLineType
    {
        Unknown = 0,
        Character = 1,
        Dialogue = 2,
        Link = 3,
        Quote = 4,
        Tag = 5,
    }

    [Serializable]
    public class MarkDialogueScriptLine
    {
        public MarkDialogueScriptLineType type = MarkDialogueScriptLineType.Unknown;
        public string rawLine = "";
    }
}
