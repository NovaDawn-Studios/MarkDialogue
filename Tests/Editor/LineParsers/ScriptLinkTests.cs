using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class ScriptLinkTests
    {
        [Test]
        public void BasicParse()
        {
            var line = MDLink.FromScriptLine("[[Some Other Script]]", 1);
            Assert.AreEqual("Some Other Script", line.TargetScript);
        }

        [Test]
        public void ParseWithDisplayValue()
        {
            var line = MDLink.FromScriptLine("[[Some Other Script|Display Text]]", 1);
            Assert.AreEqual("Some Other Script", line.TargetScript);
            Assert.AreEqual("Display Text", line.DisplayName);
        }
    }
}
