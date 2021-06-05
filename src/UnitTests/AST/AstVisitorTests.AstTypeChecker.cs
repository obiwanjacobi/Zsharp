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
                VisitChildren(expression);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                operand.TypeReference.Should().NotBeNull();
                VisitChildren(operand);
            }
            public override void VisitFunctionDefinition(AstFunctionDefinition function)
            {
                function.FunctionType.TypeReference.Should().NotBeNull();
                VisitChildren(function);
            }
            public override void VisitFunctionReference(AstFunctionReference function)
            {
                function.FunctionType.TypeReference.Should().NotBeNull();
                VisitChildren(function);
            }
            public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
            {
                parameter.TypeReference.Should().NotBeNull();
                VisitChildren(parameter);
            }
            public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
            {
                parameter.TypeReference.Should().NotBeNull();
                VisitChildren(parameter);
            }
            public override void VisitTypeReference(AstTypeReference type)
            {
                type.TypeDefinition.Should().NotBeNull();
                VisitChildren(type);
            }
            public override void VisitVariableDefinition(AstVariableDefinition variable)
            {
                variable.TypeReference.Should().NotBeNull();
                VisitChildren(variable);
            }
        };
    }
}
