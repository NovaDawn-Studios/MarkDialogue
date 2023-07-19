using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;
using System;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class CommandMethodLexerTests
    {
        [Test]
        public void BasicMethod()
        {
            var commandString = "testmethod()";
            var parsed = MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.IsEmpty(parsed.Args);
        }

        [Test]
        public void MethodWith1Arg()
        {
            var commandString = "testmethod(1336)";
            var parsed = MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(1, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
        }

        [Test]
        public void MethodWith2Arg()
        {
            var commandString = "testmethod(1336, this is a success)";
            var parsed = MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(2, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
            Assert.AreEqual("this is a success", parsed.Args[1]);
        }

        [Test]
        public void MethodWith3Arg()
        {
            var commandString = "testmethod(1336, this is a success   ,   true   )";
            var parsed = MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(3, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
            Assert.AreEqual("this is a success", parsed.Args[1]);
            Assert.AreEqual("true", parsed.Args[2]);
        }

        [Test]
        public void MethodWithQuotedArgs()
        {
            var commandString = "testmethod(1336, this is a success, \" this is a quoted string with, special \\\" characters   \", 42)";
            var parsed = MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(4, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
            Assert.AreEqual("this is a success", parsed.Args[1]);
            Assert.AreEqual(" this is a quoted string with, special \" characters   ", parsed.Args[2]);
            Assert.AreEqual("42", parsed.Args[3]);
        }

        [Test]
        public void MethodWithInvalidQuotedArgsThrows()
        {
            var commandString = "testmethod(1336, \" this is a quoted string with, special \\\" characters   \" and is now invalid, 42)";
            Assert.Throws<InvalidOperationException>(() => MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString));
        }

        [Test]
        public void MissingMethodNameThrows()
        {
            var commandString = "(this will throw)";
            Assert.Throws<InvalidOperationException>(() => MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString));
        }

        [Test]
        public void MalformedMethodNameThrows()
        {
            var commandString = "test method  with some   spaces   (this will throw)";
            Assert.Throws<InvalidOperationException>(() => MarkDialogueCommandMethodLexer.ParseSingleCommand(commandString));
        }
    }
}
