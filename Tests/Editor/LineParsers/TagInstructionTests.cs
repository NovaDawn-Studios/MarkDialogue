using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class TagInstructionTests
    {
        [Test]
        public void BasicParse()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "#TODO",
            };

            var line = MarkDialogueTagInstruction.FromScriptLine(scriptLine);
            Assert.AreEqual("TODO", line.Tag);
        }

        [Test]
        public void ParseWithArgs()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "#warn Some text",
            };

            var line = MarkDialogueTagInstruction.FromScriptLine(scriptLine);
            Assert.AreEqual("warn", line.Tag);
            Assert.AreEqual("Some text", line.Args);
        }
    }
}
