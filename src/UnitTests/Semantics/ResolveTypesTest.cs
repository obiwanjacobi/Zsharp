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
            var file = Build.File(code);
            var resolver = new ResolveTypes();
            resolver.Apply(file);
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

            var sym = file.CodeBlock.Symbols.GetEntry(v.Identifier.Name, AstSymbolKind.Variable);
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

            var sym = file.CodeBlock.Symbols.GetEntry(v.Identifier.Name, AstSymbolKind.Variable);
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
            var v = a.Variable as AstVariableReference;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.TypeReference.Should().NotBeNull();
            v.VariableDefinition.Should().BeNull();

            var sym = file.CodeBlock.Symbols.GetEntry(v.Identifier.Name, AstSymbolKind.Variable);
            sym.Definition.Should().BeNull();
        }
    }
}
