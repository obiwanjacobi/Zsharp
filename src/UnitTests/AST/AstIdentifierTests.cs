using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstIdentifierTests
    {
        [TestMethod]
        public void TopVariableName()
        {
            const string code =
                "a = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            var id = assign.Variable.Identifier;

            id.Should().NotBeNull();
            id.Name.Should().Be("a");
        }

        [TestMethod]
        public void FunctionName()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinition>(0);
            var id = fn.Identifier;

            id.Should().NotBeNull();
            id.Name.Should().Be("fn");
        }

        [TestMethod]
        public void FunctionTemplateDefinitionName()
        {
            const string code =
                "fn: <T>()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinition>(0);
            var id = fn.Identifier;

            id.Should().NotBeNull();
            id.Name.Should().Be("fn%1");
            id.CanonicalName.Should().Be("fn%1");
        }

        [TestMethod]
        public void FunctionTemplateReferenceName()
        {
            const string code =
                "fn<U8>()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionReference>(0);
            var id = fn.Identifier;

            id.Should().NotBeNull();
            id.Name.Should().Be("fn;U8");
            id.CanonicalName.Should().Be("fn;U8");
        }

        [TestMethod]
        public void FunctionParameterName()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var p = fn.FunctionType.Parameters.FirstOrDefault();
            var id = p.Identifier;

            id.Should().NotBeNull();
            id.Name.Should().Be("p");
        }

        [TestMethod]
        public void FunctionParameterSelf()
        {
            const string code =
                "fn: (self: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var p = fn.FunctionType.Parameters.FirstOrDefault();
            p.TypeReference.Should().NotBeNull();
            p.Identifier.Name.Should().Be("self");
        }

        [TestMethod]
        public void VariableAssignment()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var assign = fn.CodeBlock.LineAt<AstAssignment>(0);
            assign.Variable.Identifier.Name.Should().Be("v");
        }

        [TestMethod]
        public void ParameterAssignment()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "p = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var assign = fn.CodeBlock.LineAt<AstAssignment>(0);
            assign.Variable.Identifier.Name.Should().Be("p");
        }

        [TestMethod]
        public void CanonicalName()
        {
            const string code =
                "fn: (p_some_name: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "pSOMENAME = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var assign = fn.CodeBlock.LineAt<AstAssignment>(0);
            assign.Variable.Identifier.CanonicalName.Should().Be("psomename");
        }
    }
}
