using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.EmitCS
{
    [TestClass]
    public class EmitExpressionTests
    {
        [TestMethod]
        public void ExpressionCompareEqual()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 = 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareEqual");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 == 101");
        }

        [TestMethod]
        public void ExpressionCompareGreater()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 > 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareGreater");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 > 101");
        }

        [TestMethod]
        public void ExpressionCompareLesser()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 < 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareLesser");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 < 101");
        }

        [TestMethod]
        public void ExpressionCompareNotEqual()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 <> 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareNotEqual");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 != 101");
        }

        [TestMethod]
        public void ExpressionCompareGreaterEqual()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 >= 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareGreaterEqual");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 >= 101");
        }

        [TestMethod]
        public void ExpressionCompareLesserEqual()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 <= 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "ExpressionCompareLesserEqual");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("42 <= 101");
        }
    }
}
