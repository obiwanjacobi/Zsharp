using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstTypeTests
    {
        [TestMethod]
        public void FunctionType()
        {
            const string code =
                "fn: (): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinition>(0);
            var t = fn.TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionOptionalErrorType()
        {
            const string code =
                "fn: (): U8!?" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinition>(0);
            var t = fn.TypeReference;
            t.IsOptional.Should().BeTrue();
            t.IsError.Should().BeTrue();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionParameterType()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }

        [TestMethod]
        public void FunctionParameterCustomType()
        {
            const string code =
                "fn: (p: SomeType)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("SomeType");
        }

        [TestMethod]
        public void FunctionParameterOptionalCustomType()
        {
            const string code =
                "fn: (p: SomeType?)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var t = fn.Parameters.First().TypeReference;
            t.IsOptional.Should().BeTrue();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("SomeType");
        }

        [TestMethod]
        public void TopVariableType()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var a = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            var t = a.TypeReference;
            t.IsOptional.Should().BeFalse();
            t.IsError.Should().BeFalse();
            t.Identifier.Name.Should().Be("U8");
        }
    }
}
