using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            PrintErrors(compiler.Context.Errors);
            return compiler;
        }

        private static void PrintErrors(IEnumerable<AstMessage> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }

        [TestMethod]
        public void SyntaxError_Spaces()
        {
            const string code =
                "v=42" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            error.Context.Should().NotBeNull();
            error.Source.Should().NotBeNullOrEmpty();
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

        [TestMethod]
        public void FunctionUndefined()
        {
            const string code =
                "Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn()" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.First();
            error.Text.Should().Contain("undefined Function");
        }

        [TestMethod]
        public void FunctionOverloadNotFound()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn(\"42\")" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            error.Text.Should().Contain("No overload was found");
        }

        [TestMethod]
        public void FunctionReturnValueNotUsed()
        {
            const string code =
                "fn: (p: U8): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return p" + Tokens.NewLine +
                "Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn(42)" + Tokens.NewLine
                ;

            var compiler = Compile(code);
            var error = compiler.Context.Errors.Single();
            error.Text.Should().Contain("return value").And.Contain("assigned");
        }
    }
}
