using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstLiteralNumericTests
    {
        private static AstLiteralNumeric ParseNumeric(string code)
        {
            var file = Build.File(code);
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            return assign.Expression.RHS.LiteralNumeric;
        }

        [TestMethod]
        public void Binary()
        {
            const string code =
                "n = 0b0000_0000_1111_0000" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.AsUnsigned().Should().Be(0x00F0);
        }

        [TestMethod]
        public void Octal()
        {
            const string code =
                "n = 0c10" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.AsUnsigned().Should().Be(8);
        }

        [TestMethod]
        public void Decimal()
        {
            const string code =
                "n = 42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.AsUnsigned().Should().Be(42);
        }

        [TestMethod]
        public void Decimal_Prefix()
        {
            const string code =
                "n = 0d42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.AsUnsigned().Should().Be(42);
        }

        [TestMethod]
        public void Hexadecimal()
        {
            const string code =
                "n = 0x42" + Tokens.NewLine
                ;

            var num = ParseNumeric(code);
            num.AsUnsigned().Should().Be(0x42);
        }
    }
}
