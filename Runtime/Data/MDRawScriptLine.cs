#nullable enable

using System;
using System.Diagnostics;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{

    [Serializable]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public abstract class MDRawScriptLine
    {
        [field: SerializeField] public string RawLine { get; set; }
        [field: SerializeField] public int LineNumber { get; set; }

        protected MDRawScriptLine(string rawLine, int lineNumber)
        {
            RawLine = rawLine;
            LineNumber = lineNumber;
        }

        private string GetDebuggerDisplay()
        {
            return $"[Line {LineNumber}] {GetType()} => {RawLine}";
        }
    }
}
