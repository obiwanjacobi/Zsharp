using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.Emit;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitVariableTests
    {
        [TestMethod]
        public void VariableAssignment_Constant()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            // ldc 42, stloc 'a', ret
            body.Instructions.Should().HaveCount(3);
        }

        [TestMethod]
        public void VariableAssignment_ExpressionConstants()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42 + 101" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            // ldc 42, ldc 101, add, stloc 'a', ret
            body.Instructions.Should().HaveCount(5);
        }

        [TestMethod]
        public void VariableAssignment_ExpressionVariableRef()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "x: U8" + Tokens.NewLine +
                Tokens.Indent1 + "a = x + 1" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(5);
        }
    }
}
