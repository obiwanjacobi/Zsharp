using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace UnitTests.Semantics
{
    [TestClass]
    public class AstNodeClonerTests
    {
        private readonly Compiler _compiler =
            new Compiler(new ModuleLoader());

        private T Clone<T>(T node) where T : AstNode, IAstCodeBlockItem
        {
            var uut = new AstNodeCloner(_compiler.Context);
            return uut.Clone<T>(node);
        }

        #region Assertions
        private bool AssertNull<T>(T cloned, T origin)
            where T : class
        {
            if (cloned == null)
            {
                origin.Should().BeNull();
                return true;
            }
            if (origin == null)
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

            cloned.NodeType.Should().Be(origin.NodeType);

            if (origin is IAstCodeBlockItem cbi)
                AssertCodeBlockItem((IAstCodeBlockItem)cloned, cbi);

            if (origin is IAstExpressionSite es)
                AssertEquivalent(((IAstExpressionSite)cloned).Expression, es.Expression);

            if (origin is IAstIdentifierSite ids)
                AssertIdentifier((IAstIdentifierSite)cloned, ids);

            if (origin is IAstTemplateSite ts)
                AssertTemplate((IAstTemplateSite)cloned, ts);

            if (origin is IAstTypeReferenceSite trs)
                AssertEquivalent(((IAstTypeReferenceSite)cloned).TypeReference, trs.TypeReference);

            if (origin is IAstCodeBlockSite cbs)
                AssertEquivalent(((IAstCodeBlockSite)cloned).CodeBlock, cbs.CodeBlock);

            return false;
        }

        private void AssertCodeBlockItem(IAstCodeBlockItem cloned, IAstCodeBlockItem origin)
        {
            cloned.Indent.Should().Be(origin.Indent);
        }

        private void AssertIdentifier(IAstIdentifierSite cloned, IAstIdentifierSite origin)
        {
            cloned.Identifier.Should().BeEquivalentTo(origin.Identifier);
        }

        private void AssertTemplate(IAstTemplateSite cloned, IAstTemplateSite origin)
        {
            cloned.TemplateParameters.Should().BeEquivalentTo(origin.TemplateParameters);
            cloned.IsTemplate.Should().Be(origin.IsTemplate);
        }

        private void AssertEquivalent(AstCodeBlock cloned, AstCodeBlock origin)
        {
            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.Indent.Should().Be(origin.Indent);

            var clonedItems = cloned.Items.ToArray();
            var originItems = origin.Items.ToArray();
            for (int i = 0; i < originItems.Length; i++)
            {
                var clonedItem = clonedItems[i];
                var originItem = originItems[i];

                AssertCodeBlockItem(clonedItem, originItem);

                if (originItem is AstAssignment a)
                    AssertEquivalent((AstAssignment)clonedItem, a);

                if (originItem is AstFunctionDefinition fd)
                    AssertEquivalent((AstFunctionDefinition)clonedItem, fd);

                if (originItem is AstBranch b)
                    AssertEquivalent((AstBranch)clonedItem, b);

                if (originItem is AstVariableDefinition vd)
                    AssertEquivalent((AstVariableDefinition)clonedItem, vd);

                //if (originItem is AstTypeDefinition td)
                //    AssertEquivalent((AstTypeDefinition)clonedItem, td);
            }
        }

        private void AssertEquivalent(AstBranch cloned, AstBranch origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.BranchType.Should().Be(origin.BranchType);
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
            cloned.IsError.Should().Be(origin.IsError);
            cloned.IsOptional.Should().Be(origin.IsOptional);
            cloned.IsTemplateParameter.Should().Be(origin.IsTemplateParameter);
            cloned.TypeDefinition.Should().BeEquivalentTo(origin.TypeDefinition);

            // clone is proxied of origin
            //cloned.IsProxy.Should().Be(origin.IsProxy);
            //AssertEquivalent(cloned.TypeOrigin, origin.TypeOrigin);
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
            cloned.OverloadKey.Should().Be(origin.OverloadKey);
            cloned.Parameters.Should().BeEquivalentTo(origin.Parameters);
        }

        private void AssertEquivalent(AstFunctionReference cloned, AstFunctionReference origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);

            cloned.EnforceReturnValueUse.Should().Be(origin.EnforceReturnValueUse);
            cloned.OverloadKey.Should().Be(origin.OverloadKey);
            cloned.Parameters.Should().BeEquivalentTo(origin.Parameters);

            AssertEquivalent(cloned.FunctionDefinition, origin.FunctionDefinition);
        }

        private void AssertEquivalent(AstExpression cloned, AstExpression origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.Context.Should().BeEquivalentTo(origin.Context);
            cloned.NodeType.Should().Be(origin.NodeType);
            cloned.Operator.Should().Be(origin.Operator);
            cloned.Precedence.Should().Be(origin.Precedence);

            AssertEquivalent(cloned.LHS, origin.LHS);
            AssertEquivalent(cloned.RHS, origin.RHS);
        }

        private void AssertEquivalent(AstExpressionOperand cloned, AstExpressionOperand origin)
        {
            if (AssertNode(cloned, origin))
                return;

            cloned.NodeType.Should().Be(origin.NodeType);

            AssertEquivalent(cloned.FieldReference, origin.FieldReference);
            AssertEquivalent(cloned.FunctionReference, origin.FunctionReference);
            AssertEquivalent(cloned.VariableReference, origin.VariableReference);

            cloned.LiteralBoolean.Should().BeEquivalentTo(origin.LiteralBoolean);
            cloned.LiteralString.Should().BeEquivalentTo(origin.LiteralString);

            if (!AssertNull(cloned.LiteralNumeric, origin.LiteralNumeric))
            {
                cloned.LiteralNumeric.Context.Should().BeEquivalentTo(origin.LiteralNumeric.Context);
                cloned.LiteralNumeric.NodeType.Should().Be(origin.LiteralNumeric.NodeType);
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
            var origin = file.CodeBlock.ItemAt<AstVariableDefinition>(0);

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
            var origin = file.CodeBlock.ItemAt<AstAssignment>(0);

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
            var origin = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);

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
            var origin = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);

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
            var origin = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);

            var cloned = Clone(origin);

            AssertEquivalent(cloned, origin);
        }
    }
}
