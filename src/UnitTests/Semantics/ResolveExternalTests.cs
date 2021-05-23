using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveExternalTests
    {
        [TestMethod]
        public void ExternalZsharpConversion()
        {
            const string code =
                "v = U16(42)" + Tokens.NewLine
                ;

            var file = Compile.File(code, Compile.CreateModuleLoader());

            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            assign.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void ExternalFunctionVoidRet()
        {
            const string code =
                "import System.Console" + Tokens.NewLine +
                "WriteLine(\"Test\")" + Tokens.NewLine
                ;

            var file = Compile.File(code, Compile.CreateModuleLoader());

            var fn = file.CodeBlock.ItemAt<AstFunctionReference>(0);
            var typeRef = fn.TypeReference;
            typeRef.Symbol.Definition.Should().NotBeNull();
        }
    }
}
