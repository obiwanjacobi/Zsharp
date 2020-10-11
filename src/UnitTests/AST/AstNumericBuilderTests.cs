using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zlang.NET.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstNumericBuilderTests
    {
        [TestMethod]
        public void Binary()
        {
            const string code =
                "n = 0b0000_0000_1111_0000" + Tokens.NewLine
                ;

            var builder = new AstNumericBuilder();
            var num = builder.Test(Parser.ParseFile(code));
            num.AsUnsigned().Should().Be(0x00F0);
        }

        [TestMethod]
        public void Octal()
        {
            const string code =
                "n = 0c10" + Tokens.NewLine
                ;

            var builder = new AstNumericBuilder();
            var num = builder.Test(Parser.ParseFile(code));
            num.AsUnsigned().Should().Be(8);
        }

        [TestMethod]
        public void Decimal()
        {
            const string code =
                "n = 42" + Tokens.NewLine
                ;

            var builder = new AstNumericBuilder();
            var num = builder.Test(Parser.ParseFile(code));
            num.AsUnsigned().Should().Be(42);
        }

        [TestMethod]
        public void Decimal_Prefix()
        {
            const string code =
                "n = 0d42" + Tokens.NewLine
                ;

            var builder = new AstNumericBuilder();
            var num = builder.Test(Parser.ParseFile(code));
            num.AsUnsigned().Should().Be(42);
        }

        [TestMethod]
        public void Hexadecimal()
        {
            const string code =
                "n = 0x42" + Tokens.NewLine
                ;

            var builder = new AstNumericBuilder();
            var num = builder.Test(Parser.ParseFile(code));
            num.AsUnsigned().Should().Be(0x42);
        }
    }
}
