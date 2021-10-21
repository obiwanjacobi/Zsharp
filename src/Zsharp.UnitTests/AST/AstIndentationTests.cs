using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstIndentationTests
    {
        [TestMethod]
        public void TopVariableAfterFunction()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "p = 42" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var ag = file.CodeBlock.LineAt<AstAssignment>(1);
            ag.Should().NotBeNull();
            ag.Indent.Should().Be(0);
        }

        [TestMethod]
        public void TopVariableAfterStruct()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine +
                "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var ag = file.CodeBlock.LineAt<AstAssignment>(1);
            ag.Should().NotBeNull();
            ag.Indent.Should().Be(0);
        }

        [TestMethod]
        public void FunctionBody()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var ag = fn.CodeBlock.LineAt<AstAssignment>(0);
            ag.Should().NotBeNull();
            var br = fn.CodeBlock.LineAt<AstBranch>(1);
            br.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionBody_Branch()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.Should().NotBeNull();
            br.Indent.Should().Be(1);
            var ret = br.CodeBlock.LineAt<AstBranch>(0);
            ret.BranchKind.Should().Be(AstBranchKind.ExitFunction);
            ret.Indent.Should().Be(2);
        }

        [TestMethod]
        public void FunctionMultipleTop()
        {
            const string code =
                "fn1: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2()" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn1 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            fn1.Should().NotBeNull();
            var fn2 = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(1);
            fn2.Should().NotBeNull();
        }
    }
}
