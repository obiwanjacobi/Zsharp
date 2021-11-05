using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ResolveArrayTest
    {
        [TestMethod]
        public void ConstructorCapacity()
        {
            const string code =
                "arr = Array<U8>(10)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);
            var assign = file.CodeBlock.LineAt<AstAssignment>(0);
            assign.Expression.RHS.FunctionReference.Should().NotBeNull();
        }
    }
}
