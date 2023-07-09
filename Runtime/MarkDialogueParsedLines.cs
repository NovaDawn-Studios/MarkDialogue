#nullable enable

using NovaDawnStudios.MarkDialogue.Data;

namespace NovaDawnStudios.MarkDialogue
{
    public class MarkDialogueCharacter
    {
        public string CharacterIdentifier { get; set; } = "<UNKNOWN>";

        public static MarkDialogueCharacter FromScriptLine(MarkDialogueScriptLine line)
        {
            var character = new MarkDialogueCharacter();

            var match = MarkDialogueRegexCollection.characterRegex.Match(line.rawLine);
            character.CharacterIdentifier = match.Groups[1].Value;

            return character;
        }
    }

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

    public class MarkDialogueTagInstruction
    {
        public string Tag { get; set; } = "<UNKNOWN>";
        public string Command { get; set; } = "";

        public static MarkDialogueTagInstruction FromScriptLine(MarkDialogueScriptLine line)
        {
            var tagInstruction = new MarkDialogueTagInstruction();

            var match = MarkDialogueRegexCollection.tagRegex.Match(line.rawLine);
            tagInstruction.Tag = match.Groups[1].Value;
            if (match.Groups.Count > 2)
            {
                tagInstruction.Command = match.Groups[2].Value;
            }

            return tagInstruction;
        }
    }
}
