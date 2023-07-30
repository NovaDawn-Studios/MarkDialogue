using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class BasicLineTests
    {
        [Test]
        public void BasicParse()
        {
            var line = MDDialogueLine.FromScriptLine("This is some test dialoge", 1);
            Assert.AreEqual("This is some test dialoge", line.LineText);
        }
    }
}
