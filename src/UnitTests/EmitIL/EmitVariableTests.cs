using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.EmitIL;

namespace UnitTests.EmitIL
{
    [TestClass]
    public class EmitVariableTests
    {
        [TestMethod]
        public void TopVariableAssignment_Constant()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "a = 42" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(3);

            emit.SaveAs("TopVariableAssignment_Constant.dll");
        }

        [TestMethod]
        public void TopVariableAssignment_VariableRef()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "x = 42" + Tokens.NewLine +
                "a = x + 42" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(7);

            emit.SaveAs("TopVariableAssignment_VariableRef.dll");
        }

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

            emit.SaveAs("VariableAssignment_Constant.dll");
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

            emit.SaveAs("VariableAssignment_ExpressionConstants.dl");
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

            emit.SaveAs("VariableAssignment_ExpressionVariableRef.dll");
        }

        [TestMethod]
        public void VariableAssignment_StructFieldInit()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U8" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "s = MyStruct" + Tokens.NewLine +
                Tokens.Indent2 + "Id = 42" + Tokens.NewLine +
                Tokens.Indent2 + "Name = \"Hello\"" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(11);

            emit.SaveAs("VariableAssignment_StructFieldInit.dll");
        }
    }
}
