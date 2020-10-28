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
                "v = v + 1" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var vd = a.Variable as AstVariableDefinition;
            vd.Should().NotBeNull();
            var vr = a.Expression.LHS.VariableReference;
            vr.Should().NotBeNull();
            vr.VariableDefinition.Should().Be(vd);
            vr.Symbol.Should().Be(vd.Symbol);
        }
    }
}
