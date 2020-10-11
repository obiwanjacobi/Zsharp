using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeFlowTests
    {
        [TestMethod]
        public void If()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void IfElse()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void IfElseIfElse()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else if false" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void Loop()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "loop" + Tokens.NewLine +
                Tokens.Indent2 + "x = x + 1" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void While()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "loop x < 42" + Tokens.NewLine +
                Tokens.Indent2 + "x = x + 1" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
