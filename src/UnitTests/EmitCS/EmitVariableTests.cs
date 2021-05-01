using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.EmitCS
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

            var emitCode = Emit.Run(code, "TopVariableAssignment_Constant");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static System.Byte a = 42;");
        }

        [TestMethod]
        public void TopVariableAssignment_VariableRef()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "x: I32 = 42" + Tokens.NewLine +
                "a = x + 42" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "TopVariableAssignment_VariableRef");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static System.Int32 x = 42;");
        }

        [TestMethod]
        public void VariableAssignment_Constant()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "VariableAssignment_Constant");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("System.Byte a = 42;");
        }

        [TestMethod]
        public void VariableAssignment_ExpressionConstants()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42 + 101" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "VariableAssignment_ExpressionConstants");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("System.Byte a = 42 + 101;");
        }

        [TestMethod]
        public void VariableAssignment_ExpressionVariableRef()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "x: I32" + Tokens.NewLine +
                Tokens.Indent1 + "a = x + 1" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "VariableAssignment_ExpressionVariableRef");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("System.Int32 a = x + 1;");
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

            var emitCode = Emit.Run(code, "VariableAssignment_StructFieldInit");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("Mystruct s = new Mystruct")
                .And.Contain("Id = 42,")
                .And.Contain("Name = \"Hello\",");
        }
    }
}
