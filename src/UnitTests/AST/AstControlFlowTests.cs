using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstControlFlowTests
    {
        [TestMethod]
        public void If()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.Should().NotBeNull();
            br.BranchKind.Should().Be(AstBranchKind.Conditional);
            br.IsConditional.Should().BeTrue();
            br.HasCode.Should().BeTrue();
            br.HasExpression.Should().BeTrue();
        }

        [TestMethod]
        public void IfTrue()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.Should().NotBeNull();
            br.BranchKind.Should().Be(AstBranchKind.Conditional);
            br.IsConditional.Should().BeTrue();
            br.HasCode.Should().BeTrue();
            br.HasExpression.Should().BeTrue();
        }

        [TestMethod]
        public void Else()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.HasSubBranch.Should().BeTrue();

            var sbr = br.SubBranch;
            sbr.Should().NotBeNull();
            sbr.BranchKind.Should().Be(AstBranchKind.Conditional);
            sbr.IsConditional.Should().BeTrue();
            sbr.HasCode.Should().BeTrue();
            sbr.HasExpression.Should().BeFalse();
        }

        [TestMethod]
        public void ElseIf()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else if c = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.HasSubBranch.Should().BeTrue();

            var sbr = br.SubBranch;
            sbr.Should().NotBeNull();
            sbr.BranchKind.Should().Be(AstBranchKind.Conditional);
            sbr.IsConditional.Should().BeTrue();
            sbr.HasCode.Should().BeTrue();
            sbr.HasExpression.Should().BeTrue();
        }

        [TestMethod]
        public void ElseIfElse()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else if c = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "else" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.HasSubBranch.Should().BeTrue();

            var sbr = br.SubBranch;
            sbr.Should().NotBeNull();
            sbr.BranchKind.Should().Be(AstBranchKind.Conditional);
            sbr.IsConditional.Should().BeTrue();
            sbr.HasCode.Should().BeTrue();
            sbr.HasExpression.Should().BeTrue();

            sbr = sbr.SubBranch;
            sbr.Should().NotBeNull();
            sbr.CodeBlock.Should().NotBeNull();
            sbr.HasExpression.Should().BeFalse();
        }

        [TestMethod]
        public void IfNested()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent2 + "if c = 0" + Tokens.NewLine +
                Tokens.Indent3 + "return" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchConditional>(0);
            br.HasSubBranch.Should().BeFalse();

            var nbr = br.CodeBlock.LineAt<AstBranchConditional>(0);
            nbr.Should().NotBeNull();
            nbr.BranchKind.Should().Be(AstBranchKind.Conditional);
            nbr.IsConditional.Should().BeTrue();
            nbr.HasCode.Should().BeTrue();
            nbr.HasExpression.Should().BeTrue();
        }

        [TestMethod]
        public void Return()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranch>(0);
            br.Should().NotBeNull();
            br.BranchKind.Should().Be(AstBranchKind.ExitFunction);
        }

        [TestMethod]
        public void ReturnValue()
        {
            const string code =
                "fn: (): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.FirstOrDefault();
            var br = fn.CodeBlock.LineAt<AstBranchExpression>(0);
            br.Should().NotBeNull();
            br.BranchKind.Should().Be(AstBranchKind.ExitFunction);
            br.Expression.Should().NotBeNull();
        }

    }
}
