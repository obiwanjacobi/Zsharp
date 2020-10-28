using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveTypesTest
    {
        private static AstFile ParseFile(string code)
        {
            var errors = new AstErrorSite();

            var file = Build.File(code);
            var resolveSymbols = new ResolveSymbols(errors);
            resolveSymbols.Visit(file);
            var resolver = new ResolveTypes(errors);
            resolver.Visit(file);

            errors.HasErrors.Should().BeFalse();
            return file;
        }

        [TestMethod]
        public void TopVariableTypeDef()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var v = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            v.TypeReference.Should().NotBeNull();
            v.TypeReference.TypeDefinition.Should().NotBeNull();
            v.TypeReference.TypeDefinition.IsIntrinsic.Should().BeTrue();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableTypeDefInit()
        {
            const string code =
                "v: U8 = 42" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.TypeReference.Should().NotBeNull();
            v.TypeReference.TypeDefinition.Should().NotBeNull();
            v.TypeReference.TypeDefinition.IsIntrinsic.Should().BeTrue();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableInferType()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.FindEntry(v.Identifier.Name, AstSymbolKind.Variable);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableReference()
        {
            const string code =
                "v = 42" + Tokens.NewLine +
                "x = v" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionParameterReference()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "x = p" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var a = fn.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            var p = a.Expression.RHS.VariableReference.ParameterDefinition;
            p.Should().NotBeNull();
            v.TypeReference.Identifier.Name.Should().Be(p.TypeReference.Identifier.Name);
        }
    }
}
