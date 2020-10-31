using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstErrorTests
    {
        private static Compiler Compile(string code)
        {
            var moduleLoader = new ModuleLoader();
            var compiler = new Compiler(moduleLoader);
            compiler.Compile("UnitTests", "AstErrorTests", code);
            return compiler;
        }

        [TestMethod]
        public void TopUndefinedVariable()
        {
            const string code =
                "v = x + 42" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            error.Node.Should().BeOfType<AstVariableReference>();
            error.Text.Should().Contain("Undefined Variable");
        }
    }
}
