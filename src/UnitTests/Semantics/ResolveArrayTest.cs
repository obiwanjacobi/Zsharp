using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace UnitTests.Semantics
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

            var file = Compile.File(code, Compile.CreateModuleLoader());
            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);
            assign.Expression.RHS.FunctionReference.Should().NotBeNull();
        }
    }
}
