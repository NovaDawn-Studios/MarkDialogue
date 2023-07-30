using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class CharacterLineTests
    {
        [Test]
        public void BasicParse()
        {
            var line = MDCharacter.FromScriptLine("TESTCHAR", 1);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.IsEmpty(line.Alias);
            Assert.IsEmpty(line.Attributes);
        }

        [Test]
        public void NameWithAlias()
        {
            var line = MDCharacter.FromScriptLine("TESTCHAR as Aliased", 1);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.AreEqual("Aliased", line.Alias);
            Assert.IsEmpty(line.Attributes);
        }

        [Test]
        public void NameWithAttributes()
        {
            var line = MDCharacter.FromScriptLine("TESTCHAR - Happy, anim:clap_hands", 1);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.IsEmpty(line.Alias);
            Assert.AreEqual(new[] { "Happy", "anim:clap_hands" }, line.Attributes);
        }


        [Test]
        public void NameWithAliasAndAttributes()
        {
            var line = MDCharacter.FromScriptLine("TESTCHAR as Aliased - Happy, anim:clap_hands", 1);
            Assert.AreEqual("TESTCHAR", line.CharacterIdentifier);
            Assert.AreEqual("Aliased", line.Alias);
            Assert.AreEqual(new[] { "Happy", "anim:clap_hands" }, line.Attributes);
        }
    }
}
