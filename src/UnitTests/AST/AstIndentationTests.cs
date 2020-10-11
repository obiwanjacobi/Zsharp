using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zlang.NET.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstIndentationTests
    {
        [TestMethod]
        public void FunctionBody()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var ag = fn.CodeBlock.ItemAt<AstAssignment>(0);
            ag.Should().NotBeNull();
            var br = fn.CodeBlock.ItemAt<AstBranch>(1);
            br.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionBody_Empty()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                "v = 42" + Tokens.NewLine
                ;

            var builder = new AstBuilder();
            builder.BuildFile("", Parser.ParseFile(code));
            builder.HasErrors.Should().BeTrue();
            var err = builder.Errors.Single();
            err.Text.Should().Be(AstError.EmptyCodeBlock);
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
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var br = fn.CodeBlock.ItemAt<AstBranchConditional>(0);
            br.Should().NotBeNull();
            br.Indent.Should().Be(1);
            var ret = br.CodeBlock.ItemAt<AstBranch>(0);
            ret.BranchType.Should().Be(AstBranchType.ExitFunction);
            ret.Indent.Should().Be(2);
        }
    }
}
