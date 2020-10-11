using FluentAssertions;
using Zlang.NET.AST;

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
                if (operand.VariableReference == null)
                {     // variable not implemented
                    operand.TypeReference.Should().NotBeNull();
                }
                VisitChildren(operand);
            }
            public override void VisitFunction(AstFunction function)
            {
                function.TypeReference.Should().NotBeNull();
                VisitChildren(function);
            }
            public override void VisitFunctionParameter(AstFunctionParameter parameter)
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
