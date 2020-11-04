using FluentAssertions;
using Zsharp.AST;

namespace UnitTests.AST
{
    partial class AstVisitorTests
    {
        private class AstParentChecker : AstVisitor
        {
            public override void VisitAssignment(AstAssignment assign)
            {
                assign.Parent.Should().NotBeNull();
                VisitChildren(assign);
            }
            public override void VisitBranch(AstBranch branch)
            {
                branch.Parent.Should().NotBeNull();
                VisitChildren(branch);
            }
            public override void VisitBranchExpression(AstBranchExpression branch)
            {
                branch.Parent.Should().NotBeNull();
                VisitChildren(branch);
            }
            public override void VisitBranchConditional(AstBranchConditional branch)
            {
                branch.Parent.Should().NotBeNull();
                VisitChildren(branch);
            }
            public override void VisitCodeBlock(AstCodeBlock codeBlock)
            {
                codeBlock.Parent.Should().NotBeNull();
                VisitChildren(codeBlock);
            }
            public override void VisitExpression(AstExpression expression)
            {
                expression.Parent.Should().NotBeNull();
                VisitChildren(expression);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                operand.Parent.Should().NotBeNull();
                VisitChildren(operand);
            }
            public override void VisitFile(AstFile file)
            {
                // file is root in our tests (Parent is null)
                VisitChildren(file);
            }
            public override void VisitFunctionDefinition(AstFunctionDefinitionImpl function)
            {
                function.Parent.Should().NotBeNull();
                VisitChildren(function);
            }
            public override void VisitFunctionParameter(AstFunctionParameter parameter)
            {
                parameter.Parent.Should().NotBeNull();
                VisitChildren(parameter);
            }
            public override void VisitModulePublic(AstModulePublic module)
            {
                module.Parent.Should().NotBeNull();
                VisitChildren(module);
            }
            public override void VisitNumeric(AstNumeric numeric)
            {
                numeric.Parent.Should().NotBeNull();
                VisitChildren(numeric);
            }
            public override void VisitTypeReference(AstTypeReference type)
            {
                type.Parent.Should().NotBeNull();
                VisitChildren(type);
            }
            public override void VisitTypeDefinition(AstTypeDefinition type)
            {
                type.Parent.Should().NotBeNull();
                VisitChildren(type);
            }
            public override void VisitVariableDefinition(AstVariableDefinition variable)
            {
                variable.Parent.Should().NotBeNull();
                VisitChildren(variable);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                variable.Parent.Should().NotBeNull();
                VisitChildren(variable);
            }
        };
    }
}
