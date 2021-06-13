using FluentAssertions;
using Zsharp.AST;

namespace UnitTests.AST
{
    partial class AstVisitorTests
    {
        private class AstTypeChecker : AstVisitor
        {
            public override void VisitExpression(AstExpression expression)
            {
                expression.TypeReference.Should().NotBeNull();
                expression.VisitChildren(this);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                operand.TypeReference.Should().NotBeNull();
                operand.VisitChildren(this);
            }
            public override void VisitFunctionDefinition(AstFunctionDefinition function)
            {
                function.FunctionType.TypeReference.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionReference(AstFunctionReference function)
            {
                function.FunctionType.TypeReference.Should().NotBeNull();
                function.VisitChildren(this);
            }
            public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
            {
                parameter.TypeReference.Should().NotBeNull();
                parameter.VisitChildren(this);
            }
            public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
            {
                parameter.TypeReference.Should().NotBeNull();
                parameter.VisitChildren(this);
            }
            public override void VisitTypeReferenceType(AstTypeReferenceType type)
            {
                type.TypeDefinition.Should().NotBeNull();
                type.VisitChildren(this);
            }
            public override void VisitVariableDefinition(AstVariableDefinition variable)
            {
                variable.TypeReference.Should().NotBeNull();
                variable.VisitChildren(this);
            }
        };
    }
}
