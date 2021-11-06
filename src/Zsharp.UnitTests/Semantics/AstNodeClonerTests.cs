using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace Zsharp.UnitTests.Semantics
{
    [TestClass]
    public class AstNodeClonerTests
    {
        private readonly Compiler _compiler = new(new ModuleLoader());

        private T Clone<T>(T node) where T : AstNode, IAstCodeBlockLine
        {
            var uut = new AstNodeCloner(_compiler.Context);
            return uut.Clone<T>(node);
        }

        #region Assertions
        private bool AssertNull<T>(T cloned, T origin)
            where T : class
        {
            if (cloned is null)
            {
                origin.Should().BeNull();
                return true;
            }
            if (origin is null)
            {
                cloned.Should().BeNull();
                return true;
            }
            return false;
        }

        private bool AssertNode(AstNode cloned, AstNode origin)
        {
            if (AssertNull(cloned, origin))
                return true;

            Object.ReferenceEquals(cloned, origin).Should().BeFalse();

            cloned.NodeKind.Should().Be(origin.NodeKind);

            if (origin is IAstCodeBlockLine cbl)
                AssertCodeBlockItem((IAstCodeBlockLine)cloned, cbl);

            if (origin is IAstExpressionSite es)
                AssertEquivalent(((IAstExpressionSite)cloned).Expression, es.Expression);

            if (origin is IAstIdentifierSite ids)
                AssertIdentifier((IAstIdentifierSite)cloned, ids);

            if (origin is IAstTemplateSite<AstTemplateParameterDefinition> tpd)
                AssertTemplate((IAstTemplateSite<AstTemplateParameterDefinition>)cloned, tpd);

            if (origin is IAstTemplateSite<AstTemplateParameterReference> tpr)
                AssertTemplate((IAstTemplateSite<AstTemplateParameterReference>)cloned, tpr);

            if (origin is IAstTypeReferenceSite trs)
                AssertEquivalent(((IAstTypeReferenceSite)cloned).TypeReference, trs.TypeReference);

            if (origin is IAstCodeBlockSite cbs && cbs.HasCodeBlock)
                AssertEquivalent(((IAstCodeBlockSite)cloned).CodeBlock, cbs.CodeBlock);

            return false;
        }

        private void AssertCodeBlockItem(IAstCodeBlockLine cloned, IAstCodeBlockLine origin)
        {
            cloned.Indent.Should().Be(origin.Indent);
        }

        private void AssertIdentifier(IAstIdentifierSite cloned, IAstIdentifierSite origin)
        {
            cloned.Identifier.Should().BeEquivalentTo(origin.Identifier);
        }

        private void AssertTemplate<T>(IAstTemplateSite<T> cloned, IAstTemplateSite<T> origin)
            where T : AstTemplateParameter
        {
            cloned.TemplateParameters.Should().BeEquivalentTo(origin.TemplateParameters);
            cloned.IsTemplate.Should().Be(origin.IsTemplate);
        }

        private void AssertEquivalent(AstCodeBlock cloned, AstCodeBlock origin)
        {
            // branch does not always have a code block.
            if (AssertNull(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.Indent.Should().Be(origin.Indent);

            var clonedLines = cloned.Lines.ToArray();
            var originLines = origin.Lines.ToArray();
            for (int i = 0; i < originLines.Length; i++)
            {
                var clonedLine = clonedLines[i];
                var originLine = originLines[i];

                AssertCodeBlockItem(clonedLine, originLine);

                if (originLine is AstAssignment a)
                    AssertEquivalent((AstAssignment)clonedLine, a);

                if (originLine is AstFunctionDefinition fd)
                    AssertEquivalent((AstFunctionDefinition)clonedLine, fd);

                if (originLine is AstBranch b)
                    AssertEquivalent((AstBranch)clonedLine, b);

                if (originLine is AstVariableDefinition vd)
                    AssertEquivalent((AstVariableDefinition)clonedLine, vd);

                //if (originItem is AstTypeDefinition td)
                //    AssertEquivalent((AstTypeDefinition)clonedItem, td);
            }
        }

        private void AssertEquivalent(AstBranch cloned, AstBranch origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.BranchKind.Should().Be(origin.BranchKind);
            cloned.Context.Should().Be(origin.Context);
            cloned.IsConditional.Should().Be(origin.IsConditional);
            cloned.IsExpression.Should().Be(origin.IsExpression);

            if (origin.IsExpression)
            {
                var clonedExp = (AstBranchExpression)cloned;
                var originExp = (AstBranchExpression)origin;

                AssertEquivalent(clonedExp.Expression, originExp.Expression);
            }

            if (origin.IsConditional)
            {
                var clonedCond = (AstBranchConditional)cloned;
                var originCond = (AstBranchConditional)origin;

                AssertEquivalent(clonedCond.CodeBlock, originCond.CodeBlock);
                AssertEquivalent(clonedCond.SubBranch, originCond.SubBranch);
            }
        }

        private void AssertEquivalent(AstVariableDefinition cloned, AstVariableDefinition origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.Symbol.Definition.Should().Be(cloned);
        }

        private void AssertEquivalent(AstVariable cloned, AstVariable origin)
        {
            if (AssertNode(cloned, origin))
                return;

            if (origin is AstVariableDefinition varDef)
            {
                AssertEquivalent((AstVariableDefinition)cloned, varDef);
                return;
            }
            if (origin is AstVariableReference varRef)
            {
                AssertEquivalent((AstVariableReference)cloned, varRef);
                return;
            }
            Assert.Fail();
        }

        private void AssertEquivalent(AstVariableReference cloned, AstVariableReference origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.Field.Should().BeEquivalentTo(origin.Field);
            cloned.HasDefinition.Should().Be(origin.HasDefinition);
            cloned.ParameterDefinition.Should().BeEquivalentTo(origin.ParameterDefinition);

            AssertEquivalent(cloned.VariableDefinition, origin.VariableDefinition);
        }

        private void AssertEquivalent(AstAssignment cloned, AstAssignment origin)
        {
            if (!AssertNode(cloned, origin))
            {
                cloned.Context.Should().BeEquivalentTo(origin.Context);

                AssertEquivalent(cloned.Variable, origin.Variable);
            }
        }

        private void AssertEquivalent(AstTypeReference cloned, AstTypeReference origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.TypeDefinition.Should().BeEquivalentTo(origin.TypeDefinition);

            // clone is proxied of origin
            //cloned.IsProxy.Should().Be(origin.IsProxy);
            //AssertEquivalent(cloned.TypeOrigin, origin.TypeOrigin);

            AssertEquivalent(cloned as AstTypeReferenceTemplate, origin as AstTypeReferenceTemplate);
        }

        private void AssertEquivalent(AstTypeReferenceTemplate cloned, AstTypeReferenceTemplate origin)
        {
            if (cloned is not null &&
                origin is not null)
            {
                cloned.IsTemplateParameter.Should().Be(origin.IsTemplateParameter);
            }
        }

        private void AssertEquivalent(AstTypeFieldReference cloned, AstTypeFieldReference origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
        }

        private void AssertEquivalent(AstFunctionDefinition cloned, AstFunctionDefinition origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.IsIntrinsic.Should().Be(origin.IsIntrinsic);
            cloned.FunctionType.OverloadKey.Should().Be(origin.FunctionType.OverloadKey);
            cloned.FunctionType.Parameters.Should().BeEquivalentTo(origin.FunctionType.Parameters);
        }

        private void AssertEquivalent(AstFunctionReference cloned, AstFunctionReference origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);

            cloned.EnforceReturnValueUse.Should().Be(origin.EnforceReturnValueUse);
            cloned.FunctionType.OverloadKey.Should().Be(origin.FunctionType.OverloadKey);
            cloned.FunctionType.Parameters.Should().BeEquivalentTo(origin.FunctionType.Parameters);

            AssertEquivalent(cloned.FunctionDefinition, origin.FunctionDefinition);
        }

        private void AssertEquivalent(AstExpression cloned, AstExpression origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.NodeKind.Should().Be(origin.NodeKind);
            cloned.Operator.Should().Be(origin.Operator);
            cloned.Precedence.Should().Be(origin.Precedence);

            AssertEquivalent(cloned.LHS, origin.LHS);
            AssertEquivalent(cloned.RHS, origin.RHS);
        }

        private void AssertEquivalent(AstExpressionOperand cloned, AstExpressionOperand origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.NodeKind.Should().Be(origin.NodeKind);

            AssertEquivalent(cloned.FieldReference, origin.FieldReference);
            AssertEquivalent(cloned.FunctionReference, origin.FunctionReference);
            AssertEquivalent(cloned.VariableReference, origin.VariableReference);

            cloned.LiteralBoolean.Should().BeEquivalentTo(origin.LiteralBoolean);
            cloned.LiteralString.Should().BeEquivalentTo(origin.LiteralString);

            if (!AssertNull(cloned.LiteralNumeric, origin.LiteralNumeric))
            {
                cloned.LiteralNumeric.Context.Should().BeEquivalentTo(origin.LiteralNumeric.Context);
                cloned.LiteralNumeric.NodeKind.Should().Be(origin.LiteralNumeric.NodeKind);
                cloned.LiteralNumeric.Sign.Should().Be(origin.LiteralNumeric.Sign);
                cloned.LiteralNumeric.Value.Should().Be(origin.LiteralNumeric.Value);
            }
        }
        #endregion // Assertions

        [TestMethod]
        public void TopVariableDefinition()
        {
            const string code =
                "v: U8" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var origin = file.CodeBlock.LineAt<AstVariableDefinition>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }

        [TestMethod]
        public void TopVariableAssignment()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var origin = file.CodeBlock.LineAt<AstAssignment>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }

        [TestMethod]
        public void FunctionDefinition()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var origin = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }

        [TestMethod]
        public void VariableDefinition()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v: U8" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var origin = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }

        [TestMethod]
        public void VariableAssignment()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Compile.File(code);
            var origin = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }
    }
}
