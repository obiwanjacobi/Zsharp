using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Grammar
{
    [TestClass]
    public class GrammarExpressionTests
    {
        [TestMethod]
        public void ArithemeticLiterals()
        {
            const string code = "x = 2 + 3" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ArithemeticLiteralVariable()
        {
            const string code = "x = 2 + a" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ArithemeticVariables()
        {
            const string code = "x = a + b" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ComparisonLiteralVariable()
        {
            const string code = "b = 2 < a" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ComparisonLogicalLiteralVariable()
        {
            const string code = "b = 2 < a and 10 > b" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ArithmeticComparisonLogicalLiteralVariable()
        {
            const string code = "b = not ((2 + 7) / 8) > 4" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void BitwiseArithmetic()
        {
            const string code = "b = 42 * (x >> 7) / 4" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
