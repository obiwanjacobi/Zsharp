using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ResolveSymbolsTests
    {
        [TestMethod]
        public void FunctionReturnIntrinsic()
        {
            const string code =
                "fn: (): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"Hello Z#\"" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var strSymbol = file.CodeBlock.Symbols.FindSymbol("Str", Zsharp.AST.AstSymbolKind.Type);

            strSymbol.Definition.Should().NotBeNull();
        }
    }
}
