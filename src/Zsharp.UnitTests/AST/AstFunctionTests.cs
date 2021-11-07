using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstFunctionTests
    {
        [TestMethod]
        public void FunctionTypeDefinition_Name()
        {
            const string code =
                "fn: (p: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "return false" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            fn.Should().NotBeNull();

            fn.FunctionType.Identifier.CanonicalFullName.Should().Be("(U8): Bool");
        }
    }
}
