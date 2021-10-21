using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.EmitCS
{
    [TestClass]
    public class EmitBranchTests
    {
        [TestMethod]
        public void BranchIfTrue()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "BranchIfTrue");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("if (");
        }

        [TestMethod]
        public void BranchIfCompare()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U32)" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "BranchIfCompare");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("if (");
        }

        [TestMethod]
        public void BranchIfElse()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U32)" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "BranchIfElse");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("if (")
                .And.Contain("else");
        }

        [TestMethod]
        public void BranchIfElseIf()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U32)" + Tokens.NewLine +
                Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else if p = 0" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "BranchIfElseIf");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("if (")
                .And.Contain("else if");
        }
    }
}
