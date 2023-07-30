#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public sealed class MDScriptAsset : ScriptableObject
    {
        internal const string DEFAULT_SCRIPT_NAME = "Default Script";

        /// <summary>
        ///     The script collection that owns this script.
        /// </summary>
        [field: SerializeField] public MDScriptCollectionAsset ParentCollection { get; set; } = default!;

        /// <summary>
        ///     The file path to this script. Used to find other scripts via <c>[[Link]]</c> tags.
        /// </summary>
        [field: SerializeField] public string AssetPath { get; set; } = "";

        /// <summary>
        ///     The collection of lines in this script.
        /// </summary>
        [field: SerializeReference] public List<MDRawScriptLine> Lines { get; set; } = new();

        /// <summary>
        ///     The starting line in the script collection where this script starts.
        /// </summary>
        [field: SerializeField] public int StartLine { get; set; } = 0;

        /// <summary>
        ///     The endling line in the script collection where this script ends.
        /// </summary>
        [field: SerializeField] public int EndLine { get; set; } = 0;

        /// <summary>
        ///     The collection of lines in this script.
        /// </summary>
        [field: SerializeField] public List<MDScriptAsset> LinkedToScripts { get; set; } = new();

        /// <summary>
        ///     Parses the supplied pre-split script lines starting from <paramref name="lineNumber"/>. Returns the last line we get to before ending, which is either
        ///     the last line in the file, or the heading of the next script in a multi-script file.
        /// </summary>
        /// <param name="parentCollection">The collection that owns this script.</param>
        /// <param name="splScript">The lines to parse.</param>
        /// <param name="lineNumber">The starting line</param>
        /// <returns>The last line that we parsed to - either the end of the file, or the heading that starts a new script.</returns>
        public int Parse(MDScriptCollectionAsset parentCollection, string[] splScript, int lineNumber)
        {
            ParentCollection = parentCollection;
            AssetPath = parentCollection.AssetPath;
            StartLine = EndLine = lineNumber;

            // Check if we have a section name
            string line = splScript[lineNumber].Trim();
            var match = MDRegexCollection.headingRegex.Match(line);
            if (match.Success)
            {
                name = match.Groups[1].Value;
                AssetPath += $"#{name}";
                ++lineNumber;
            }
            else
            {
                name = DEFAULT_SCRIPT_NAME;
            }

            for (; lineNumber < splScript.Length; ++lineNumber)
            {
                EndLine = lineNumber;
                line = splScript[lineNumber].Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (MDRegexCollection.headingRegex.IsMatch(line)) // We've hit a new heading. Bounce out and let the next MarkDialogueScript handle it.
                {
                    break;
                }

                if (MDRegexCollection.linkRegex.IsMatch(line))
                {
                    Lines.Add(MDLink.FromScriptLine(line, lineNumber));
                    continue;
                }

                if (line[0] == '#') // A safe check. The above heading Regex would have caught '# Anything', so this MUST be '#Anything' with no whitespace
                {
                    Lines.Add(MDTagInstruction.FromScriptLine(line, lineNumber));
                    continue;
                }

                if (line[0] == '>')
                {
                    Lines.Add(MDQuoteLine.FromScriptLine(line, lineNumber));
                    continue;
                }
                
                match = MDRegexCollection.characterRegex.Match(line);
                if (match.Success)
                {
                    Lines.Add(MDCharacter.FromScriptLine(line, lineNumber));
                    continue;
                }

                // Must be a dialogue line, or something currently unhandled. Presume the best case scenario.
                Lines.Add(MDDialogueLine.FromScriptLine(line, lineNumber));
            }

            return lineNumber;
        }
    }
}
