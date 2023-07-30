using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class TagInstructionTests
    {
        [Test]
        public void BasicParse()
        {
            var line = MDTagInstruction.FromScriptLine("#TODO", 1);
            Assert.AreEqual("TODO", line.Tag);
        }

        [Test]
        public void ParseWithArgs()
        {
            var line = MDTagInstruction.FromScriptLine("#warn Some text", 1);
            Assert.AreEqual("warn", line.Tag);
            Assert.AreEqual("Some text", line.Args);
        }
    }
}
