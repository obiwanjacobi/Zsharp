using FluentAssertions;
using Zsharp.AST;

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
                    operand.LiteralNumeric == null &&
                    operand.FunctionReference == null)
                {
                    operand.VariableReference.Should().NotBeNull();
                }
                VisitChildren(operand);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                VisitChildren(variable);
            }
        }
    }
}
