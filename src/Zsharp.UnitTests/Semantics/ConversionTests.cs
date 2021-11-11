using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void IntrinsicConversion()
        {
            const string code =
                "fn: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var intrinsic = fn.SymbolTable.FindDefinition<AstFunctionDefinition>("U16", AstSymbolKind.Function);
            intrinsic.FunctionType.Parameters.First().IsSelf.Should().BeTrue();
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

            var file = Compile.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(2);
            var intrinsic = fn.SymbolTable.FindDefinition<AstFunctionDefinition>("U16", AstSymbolKind.Function);
            intrinsic.FunctionType.Parameters.First().IsSelf.Should().BeTrue();
        }
    }
}
