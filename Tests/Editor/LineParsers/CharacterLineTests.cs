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
            Assert.IsEmpty(line.Alias);
            Assert.IsEmpty(line.Attributes);
        }

        [Test]
        public void NameWithAlias()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "TESTCHAR as Aliased",
            };

            var line = MarkDialogueCharacter.FromScriptLine(scriptLine);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.AreEqual("Aliased", line.Alias);
            Assert.IsEmpty(line.Attributes);
        }

        [Test]
        public void NameWithAttributes()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "TESTCHAR - Happy, anim:clap_hands",
            };

            var line = MarkDialogueCharacter.FromScriptLine(scriptLine);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.IsEmpty(line.Alias);
            Assert.AreEqual(new[] { "Happy", "anim:clap_hands" }, line.Attributes);
        }


        [Test]
        public void NameWithAliasAndAttributes()
        {
            var scriptLine = new MarkDialogueScriptLine()
            {
                rawLine = "TESTCHAR as Aliased - Happy, anim:clap_hands",
            };

            var line = MarkDialogueCharacter.FromScriptLine(scriptLine);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.AreEqual("Aliased", line.Alias);
            Assert.AreEqual(new[] { "Happy", "anim:clap_hands" }, line.Attributes);
        }
    }
}
