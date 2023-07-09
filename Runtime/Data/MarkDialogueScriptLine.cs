#nullable enable

using System;
using System.Diagnostics;

namespace NovaDawnStudios.MarkDialogue.Data
{
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
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class MarkDialogueScriptLine
    {
        public MarkDialogueScriptLineType type = MarkDialogueScriptLineType.Unknown;
        public string rawLine = "";
        public int lineNumber = -1;

        private string GetDebuggerDisplay()
        {
            return $"[Line {lineNumber}] {type} => {rawLine}";
        }
    }
}
