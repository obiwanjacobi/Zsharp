using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveSymbolsTests
    {
        private static AstFile ParseFile(string code)
        {
            var errors = new AstErrorSite();

            var file = Build.File(code);
            var resolveSymbols = new ResolveSymbols(errors);
            resolveSymbols.Visit(file);

            errors.HasErrors.Should().BeFalse();
            return file;
        }

        [TestMethod]
        public void TopVariableInferDef()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TopVariableReference()
        {
            const string code =
                "x: U8" + Tokens.NewLine +
                "v = x + 1" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var x = file.CodeBlock.ItemAt<AstVariableDefinition>(0);
            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            var vd = a.Variable as AstVariableDefinition;
            vd.Should().NotBeNull();
            var vr = a.Expression.LHS.VariableReference;
            vr.Should().NotBeNull();

            vr.VariableDefinition.Should().Be(x);
            vr.Symbol.Should().Be(x.Symbol);
        }

        [TestMethod]
        public void FunctionReference()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn()" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var fn = file.CodeBlock.ItemAt<AstFunctionDefinition>(0);
            fn.Should().NotBeNull();

            var fnRef = fn.CodeBlock.ItemAt<AstFunctionReference>(0);
            fnRef.Should().NotBeNull();
            fnRef.Symbol.Definition.Should().NotBeNull();
        }
    }
}
