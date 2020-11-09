using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.Emit;

namespace UnitTests.Emit
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

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(5);

            emit.SaveAs("BranchIfTrue.dll");
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

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(7);

            emit.SaveAs("BranchIfCompare.dll");
        }
    }
}
