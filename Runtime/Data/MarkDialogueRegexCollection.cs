using System.Text.RegularExpressions;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a single contiguous MarkDialogue script.
    /// </summary>
    public sealed class MarkDialogueRegexCollection
    {
        /// <summary>
        ///     Matches an Obsidian comment in the form <c>%% some txt %%</c>. These comments are removed from the script before parsing.
        /// </summary>
        public static readonly Regex commentRegex = new Regex(@"%%.*?%%", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        ///     Matches a MarkDown heading, capturing the text of the heading and stopping before an Obsidian comment or the end of the line.
        ///     This signifies the line represents the start of a new script within the file.
        /// </summary>
        public static readonly Regex headingRegex = new(@"^#{1,6}\s+(.*?)(?=%%|$)", RegexOptions.Compiled);

        /// <summary>
        ///     Matches one or more Unicode uppercase characters, hyphens and underscores at the start of the line.
        ///     This signifies the line represents a character.
        /// </summary>
        public static readonly Regex characterRegex = new(@"^([\p{Lu}-_]+)(?=\s|$)", RegexOptions.Compiled);

        /// <summary>
        ///     Matches a link in the form <c>[[Some test]]</c> that either exists on its own line or ends with an Obsidian comment.
        /// </summary>
        public static readonly Regex linkRegex = new(@"^\[\[(.+)\]\](?=\s*%%|$)", RegexOptions.Compiled);

        /// <summary>
        ///     Matches a logic tag in the form <c>#TagName {Optional Arguments}</c>.
        /// </summary>
        public static readonly Regex tagRegex = new(@"^#(\w+)(?:\s(.*))?$", RegexOptions.Compiled);
    }
}