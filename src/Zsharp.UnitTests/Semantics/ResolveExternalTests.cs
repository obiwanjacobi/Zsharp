using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
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

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            assign.Expression.RHS.FunctionReference.FunctionDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void ExternalFunctionVoidRet()
        {
            const string code =
                "import System.Console" + Tokens.NewLine +
                "WriteLine(\"Test\")" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddSystemConsole()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var fn = file.CodeBlock.LineAt<AstFunctionReference>(0);
            var typeRef = fn.FunctionType.TypeReference;
            typeRef.Symbol.Definition.Should().NotBeNull();
        }
    }
}
