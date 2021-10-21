using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstLiteralTests
    {
        private static AstLiteralNumeric ParseNumeric(string code)
        {
            var file = Build.File(code);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            return assign.Expression.RHS.LiteralNumeric;
        }

        private static AstLiteralBoolean ParseBoolean(string code)
        {
            var file = Build.File(code);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            return assign.Expression.RHS.LiteralBoolean;
        }

        private static AstLiteralString ParseString(string code)
        {
            var file = Build.File(code);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            return assign.Expression.RHS.LiteralString;
        }

        [TestMethod]
        public void Boolean()
        {
            const string code =
                "n = true" + Tokens.NewLine
                ;

            var bl = ParseBoolean(code);
            bl.Value.Should().BeTrue();
        }

        [TestMethod]
        public void NumericBinary()
        {
            const string code =
                "n = 0b0000_0000_1111_0000" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.Value.Should().Be(0x00F0);
        }

        [TestMethod]
        public void NumericOctal()
        {
            const string code =
                "n = 0c10" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.Value.Should().Be(8);
        }

        [TestMethod]
        public void NumericDecimal()
        {
            const string code =
                "n = 42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.Value.Should().Be(42);
        }

        [TestMethod]
        public void NumericDecimal_Prefix()
        {
            const string code =
                "n = 0d42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.Value.Should().Be(42);
        }

        [TestMethod]
        public void NumericHexadecimal()
        {
            const string code =
                "n = 0x42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.Value.Should().Be(0x42);
        }

        [TestMethod]
        public void String()
        {
            const string txt =
                "Hello Z# World.";
            string code =
                $"s = \"{txt}\"" + Tokens.NewLine
                ;

            var str = ParseString(code);
            str.Value.Should().Be(txt);
        }
    }
}
