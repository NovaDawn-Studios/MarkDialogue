using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class CharacterLineTests
    {
        [Test]
        public void BasicParse()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "TESTCHAR",
            };

            var line = MarkDialogueCharacter.FromScriptLine(scriptLine);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
        }

        [Test, Ignore("TODO")]
        public void NameAlias()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "TESTCHAR as Aliased",
            };

            var line = MarkDialogueCharacter.FromScriptLine(scriptLine);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.AreEqual("Aliased", line.Alias);
        }
    }
}
