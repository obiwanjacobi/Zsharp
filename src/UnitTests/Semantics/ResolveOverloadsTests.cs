using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace UnitTests.Semantics
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

            var fn1 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.ItemAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(1);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.References.Should().HaveCount(1);
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

            var fn1 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.ItemAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(2);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.References.Should().HaveCount(1);
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

            var fn1 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var fn2Ref = fn1.CodeBlock.ItemAt<AstFunctionReference>(0);

            var fn2 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(2);
            fn2.Should().NotBeNull();
            fn2.Symbol.SymbolName.Should().Be("fn2");
            fn2.Symbol.HasOverloads.Should().BeTrue();
            fn2.Symbol.References.Should().HaveCount(1);
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

            var file = Compile.File(code, Compile.CreateModuleLoader());
            var a = file.CodeBlock.ItemAt<AstAssignment>(1);
            a.Expression.TypeReference.Should().NotBeNull();
            a.Variable.TypeReference.Should().NotBeNull();
        }

        [TestMethod]
        public void ResolveConversionFunction_Expression()
        {
            const string code =
                "fn1: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var file = Compile.File(code, Compile.CreateModuleLoader());

            var fn1 = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var br = fn1.CodeBlock.ItemAt<AstBranchExpression>(0);
            var convRef = br.Expression.RHS.FunctionReference;

            convRef.FunctionDefinition.Should().NotBeNull();
        }

        [TestMethod]
        public void ResolveConversionFunction_TopVariableAssign()
        {
            const string code =
                "x = U16(42)" + Tokens.NewLine
                ;

            var file = Compile.File(code, Compile.CreateModuleLoader());

            var assign = file.CodeBlock.ItemAt<AstAssignment>(0);

            assign.Expression.TypeReference.Should().NotBeNull();
        }
    }
}
