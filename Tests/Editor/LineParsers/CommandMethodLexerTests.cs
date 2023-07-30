using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;
using System;
using UnityEditor.VersionControl;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests.LineParsers
{
    public class CommandMethodLexerTests
    {
        [Test]
        public void BasicMethod()
        {
            var commandString = "testmethod()";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString) as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(parsed);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.IsEmpty(parsed.Args);
        }

        [Test]
        public void MethodWith1Arg()
        {
            var commandString = "testmethod(1336)";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString) as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(parsed);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(1, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
        }

        [Test]
        public void MethodWith2Arg()
        {
            var commandString = "testmethod(1336, this is a success)";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString) as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(parsed);
            Assert.AreEqual("testmethod", parsed.MethodName);
            Assert.AreEqual(2, parsed.Args.Length);
            Assert.AreEqual("1336", parsed.Args[0]);
            Assert.AreEqual("this is a success", parsed.Args[1]);
        }

        [Test]
        public void MethodWith3Arg()
        {
            var commandString = "testmethod(1336, this is a success   ,   true   )";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString) as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(parsed);
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
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString) as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(parsed);
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
            Assert.Throws<InvalidOperationException>(() => MDCommandMethodLexer.ParseSingleCommand(commandString));
        }

        [Test]
        public void MissingMethodNameThrows()
        {
            var commandString = "(this will throw)";
            Assert.Throws<InvalidOperationException>(() => MDCommandMethodLexer.ParseSingleCommand(commandString));
        }

        [Test]
        public void MalformedMethodNameThrows()
        {
            var commandString = "test method  with some   spaces   (this will throw)";
            Assert.Throws<InvalidOperationException>(() => MDCommandMethodLexer.ParseSingleCommand(commandString));
        }

        [Test]
        [TestCase("and", MDCommandMethodLexer.EMDCommandGroupComparisonType.And, TestName = "AND joiner")]
        [TestCase("or", MDCommandMethodLexer.EMDCommandGroupComparisonType.Or, TestName = "OR joiner")]
        public void ComplexCommandWithJoiner(string joiner, MDCommandMethodLexer.EMDCommandGroupComparisonType compType)
        {
            var commandString = $"testmethod() {joiner} othermethod(1)";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString);
            var group = parsed as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(group);
            Assert.AreEqual(compType, group.ComparisonType);
            var left = group.Left as MDCommandMethodLexer.MDCommand;
            var right = group.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.AreEqual("testmethod", left.MethodName);
            Assert.IsEmpty(left.Args);
            Assert.AreEqual("othermethod", right.MethodName);
            Assert.AreEqual(1, right.Args.Length);
        }

        [Test]
        public void ComplexCommandWithRightNest()
        {
            var commandString = "testmethod() and (othermethod(1) or anothermethod(1, 2))";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString);
            var group = parsed as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(group);
            var left = group.Left as MDCommandMethodLexer.MDCommand;
            var right = group.Right as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            var nestedLeft = right.Left as MDCommandMethodLexer.MDCommand;
            var nestedRight = right.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(nestedLeft);
            Assert.IsNotNull(nestedRight);

            Assert.AreEqual("testmethod", left.MethodName);
            Assert.IsEmpty(left.Args);
            Assert.AreEqual("othermethod", nestedLeft.MethodName);
            Assert.AreEqual(1, nestedLeft.Args.Length);
            Assert.AreEqual("anothermethod", nestedRight.MethodName);
            Assert.AreEqual(2, nestedRight.Args.Length);
        }

        [Test]
        public void ComplexCommandWithLeftNest()
        {
            var commandString = "(testmethod() and othermethod(1)) or anothermethod(1, 2)";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString);
            var group = parsed as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(group);
            var left = group.Left as MDCommandMethodLexer.MDCommandGroup;
            var right = group.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            var nestedLeft = left.Left as MDCommandMethodLexer.MDCommand;
            var nestedRight = left.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(nestedLeft);
            Assert.IsNotNull(nestedRight);

            Assert.AreEqual("testmethod", nestedLeft.MethodName);
            Assert.IsEmpty(nestedLeft.Args);
            Assert.AreEqual("othermethod", nestedRight.MethodName);
            Assert.AreEqual(1, nestedRight.Args.Length);
            Assert.AreEqual("anothermethod", right.MethodName);
            Assert.AreEqual(2, right.Args.Length);
        }

        [Test]
        public void ComplexCommandWithMultipleNests()
        {
            var commandString = "(a() and (b(1) or c(1,2))) and ((d(1,2,3) or e(\"a\")) and f(true))";
            var parsed = MDCommandMethodLexer.ParseCommandString(commandString);
            var group = parsed as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(group);
            var left = group.Left as MDCommandMethodLexer.MDCommandGroup;
            var right = group.Right as MDCommandMethodLexer.MDCommandGroup;
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            var leftCmd = left.Left as MDCommandMethodLexer.MDCommand;
            var leftNest = left.Right as MDCommandMethodLexer.MDCommandGroup;
            var rightNest = right.Left as MDCommandMethodLexer.MDCommandGroup;
            var rightCmd = right.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(leftCmd);
            Assert.IsNotNull(leftNest);
            Assert.IsNotNull(rightNest);
            Assert.IsNotNull(rightCmd);

            Assert.AreEqual("a", leftCmd.MethodName);
            Assert.IsEmpty(leftCmd.Args);
            Assert.AreEqual("f", rightCmd.MethodName);
            Assert.AreEqual(1, rightCmd.Args.Length);
            
            var leftNestLeft = leftNest.Left as MDCommandMethodLexer.MDCommand;
            var leftNestRight = leftNest.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(leftNestLeft);
            Assert.IsNotNull(leftNestRight);
            Assert.AreEqual("b", leftNestLeft.MethodName);
            Assert.AreEqual(1, leftNestLeft.Args.Length);
            Assert.AreEqual("c", leftNestRight.MethodName);
            Assert.AreEqual(2, leftNestRight.Args.Length);

            var rightNestLeft = rightNest.Left as MDCommandMethodLexer.MDCommand;
            var rightNestRight = rightNest.Right as MDCommandMethodLexer.MDCommand;
            Assert.IsNotNull(rightNestLeft);
            Assert.IsNotNull(rightNestRight);
            Assert.AreEqual("d", rightNestLeft.MethodName);
            Assert.AreEqual(3, rightNestLeft.Args.Length);
            Assert.AreEqual("e", rightNestRight.MethodName);
            Assert.AreEqual(1, rightNestRight.Args.Length);
        }
    }
}
