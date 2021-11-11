using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class ResolveOverloadsTests
    {
        [TestMethod]
        public void OverloadNoParams()
        {
            const string code =
                "fn1: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2()" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "fn2: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn1 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.LineAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(1);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Symbol.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.ChildSymbols.ElementAt(0).References.Should().HaveCount(1);
            fn2.Symbol.FindFunctionDefinition(fn2Ref).Should().Be(fn2);
        }

        [TestMethod]
        public void OverloadOneParam()
        {
            const string code =
                "fn1: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2(42)" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "fn2: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn1 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.LineAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(2);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Symbol.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.ChildSymbols.ElementAt(0).References.Should().HaveCount(1);
            fn2.Symbol.FindFunctionDefinition(fn2Ref).Should().Be(fn2);
        }

        [TestMethod]
        public void OverloadTwoParams()
        {
            const string code =
                "fn1: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2(42, \"42\")" + Tokens.NewLine +
                "fn2: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "fn2: (p: U8, s: Str)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Compile.File(code);

            var fn1 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.LineAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(2);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Symbol.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.ChildSymbols.ElementAt(0).References.Should().HaveCount(1);
            fn2.Symbol.FindFunctionDefinition(fn2Ref).Should().Be(fn2);
        }

        [TestMethod]
        public void ResolveFunction_Expression()
        {
            const string code =
                "fn: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine +
                "x = fn(42)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);
            var a = file.CodeBlock.LineAt<AstAssignment>(1);
            a.Expression.HasTypeReference.Should().BeTrue();
            a.Variable.HasTypeReference.Should().BeTrue();
        }

        [TestMethod]
        public void ResolveConversionFunction_Expression()
        {
            const string code =
                "fn1: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var fn1 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var br = fn1.CodeBlock.LineAt<AstBranchExpression>(0);
            var convRef = br.Expression.RHS.FunctionReference;

            convRef.FunctionDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void ResolveConversionFunction_TopVariableAssign()
        {
            const string code =
                "x = U16(42)" + Tokens.NewLine
                ;

            var moduleLoader = new AssemblyManagerBuilder()
                .AddZsharpRuntime()
                .ToModuleLoader();

            var file = Compile.File(code, moduleLoader);

            var assign = file.CodeBlock.LineAt<AstAssignment>(0);

            assign.Expression.TypeReference.Should().NotBeNull();
        }
    }
}
