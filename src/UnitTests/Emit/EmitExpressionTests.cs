using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.Emit;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitExpressionTests
    {
        [TestMethod]
        public void ExpressionCompare()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "b = 42 = 101" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(5);

            emit.SaveAs("ExpressionCompare.dll");
        }
    }
}
