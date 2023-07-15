#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using Unity.VisualScripting.YamlDotNet.Serialization;

namespace NovaDawnStudios.MarkDialogue
{
    public class MarkDialogueLine
    {
        public string LineText { get; set; } = "<UNKNOWN>";

        public static MarkDialogueLine FromScriptLine(MarkDialogueScriptLine line)
        {
            var mdLine = new MarkDialogueLine();
            mdLine.LineText = line.rawLine;
            return mdLine;
        }
    }

    public class MarkDialogueCharacter
    {
        public string CharacterIdentifier { get; set; } = "<UNKNOWN>";
        public string Alias { get; set; } = "";

        public static MarkDialogueCharacter FromScriptLine(MarkDialogueScriptLine line)
        {
            var character = new MarkDialogueCharacter();

            var match = MarkDialogueRegexCollection.characterRegex.Match(line.rawLine);
            character.CharacterIdentifier = match.Groups[1].Value;

            return character;
        }
    }

    public class MarkDialogueTagInstruction
    {
        public string Tag { get; set; } = "<UNKNOWN>";
        public string Args { get; set; } = "";

        public static MarkDialogueTagInstruction FromScriptLine(MarkDialogueScriptLine line)
        {
            var tagInstruction = new MarkDialogueTagInstruction();

            var match = MarkDialogueRegexCollection.tagRegex.Match(line.rawLine);
            tagInstruction.Tag = match.Groups["tag"].Value;
            tagInstruction.Args = match.Groups["args"].Value;

            return tagInstruction;
        }
    }

    public class MarkDialogueLink
    {
        public string TargetScript { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public static MarkDialogueLink FromScriptLine(MarkDialogueScriptLine line)
        {
            var tagInstruction = new MarkDialogueLink();

            var match = MarkDialogueRegexCollection.linkRegex.Match(line.rawLine);
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
