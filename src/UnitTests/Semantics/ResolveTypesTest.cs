using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveTypesTest
    {
        [TestMethod]
        public void TopVariableTypeDef()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = Build.File(code);

            var resolver = new ResolveTypes();
            resolver.Apply(file);

            var v = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            v.TypeReference.Should().NotBeNull();
            v.TypeReference.TypeDefinition.Should().NotBeNull();
            v.TypeReference.TypeDefinition.IsIntrinsic.Should().BeTrue();
        }

        [TestMethod]
        public void TopVariableInferType()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);

            var resolver = new ResolveTypes();
            resolver.Apply(file);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableReference;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);
            v.VariableDefinition.Should().BeNull();

            var sym = file.CodeBlock.Symbols.GetEntry(v.Identifier.Name, AstSymbolKind.Variable);
            sym.Definition.Should().BeNull();
        }
    }
}
