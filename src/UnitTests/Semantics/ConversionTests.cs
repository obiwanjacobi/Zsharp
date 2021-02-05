using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.AST
{
    [TestClass]
    public class ConversionTests
    {
        private static AstFile ParseFile(string code)
        {
            var errors = new AstErrorSite();

            var file = Build.File(code);
            var resolveSymbols = new ResolveSymbols(errors);
            resolveSymbols.Visit(file);

            errors.HasErrors.Should().BeFalse();
            return file;
        }

        [TestMethod]
        public void IntrinsicConversion()
        {
            const string code =
                "fn: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var file = ParseFile(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var intrinsic = fn.Symbols.FindDefinition<AstFunctionDefinitionIntrinsic>("U16", AstSymbolKind.Function);
            intrinsic.Parameters.First().IsSelf.Should().BeTrue();
        }

        [TestMethod]
        public void CustomConversion()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Fld: U16" + Tokens.NewLine +
                "U16: (self: MyStruct): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return self.Fld" + Tokens.NewLine +
                "fn: (p: MyStruct): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var file = ParseFile(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(2);
            var intrinsic = fn.Symbols.FindDefinition<AstFunctionDefinition>("U16", AstSymbolKind.Function);
            intrinsic.Parameters.First().IsSelf.Should().BeTrue();
        }
    }
}
