using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class ScriptLinkTests
    {
        [Test]
        public void BasicParse()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "[[Some Other Script]]",
            };

            var line = MarkDialogueLink.FromScriptLine(scriptLine);
            Assert.AreEqual("Some Other Script", line.TargetScript);
        }

        [Test]
        public void ParseWithDisplayValue()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "[[Some Other Script|Display Text]]",
            };

            var line = MarkDialogueLink.FromScriptLine(scriptLine);
            Assert.AreEqual("Some Other Script", line.TargetScript);
            Assert.AreEqual("Display Text", line.DisplayName);
        }
    }
}
