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
        public void SyntaxError()
        {
            const string code =
                "v + 42" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            error.Context.Should().NotBeNull();
            error.Source.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void TopUndefinedVariable()
        {
            const string code =
                "v = x + 42" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            var variable = (AstVariableReference)error.Node;
            variable.Identifier.Name.Should().Be("x");
            error.Text.Should().Contain("undefined Variable");
        }
    }
}
