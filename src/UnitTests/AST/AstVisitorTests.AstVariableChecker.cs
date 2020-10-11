using FluentAssertions;
using Zlang.NET.AST;

namespace UnitTests.AST
{
    partial class AstVisitorTests
    {
        public class AstVariableChecker : AstVisitor
        {
            public override void VisitAssignment(AstAssignment assign)
            {
                assign.Variable.Should().NotBeNull();
                VisitChildren(assign);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                if (operand.Expression == null &&
                    operand.Numeric == null)
                {
                    operand.VariableReference.Should().NotBeNull();
                }
                VisitChildren(operand);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                //ASSERT_NE(variable.getVariableDefinition(), nullptr);
                VisitChildren(variable);
            }
        }
    }
}
