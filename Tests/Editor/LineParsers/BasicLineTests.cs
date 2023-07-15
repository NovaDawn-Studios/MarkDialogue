using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class BasicLineTests
    {
        [Test]
        public void BasicParse()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "This is some test dialoge",
            };

            var line = MarkDialogueLine.FromScriptLine(scriptLine);
            Assert.AreEqual("This is some test dialoge", line.LineText);
        }
    }
}
