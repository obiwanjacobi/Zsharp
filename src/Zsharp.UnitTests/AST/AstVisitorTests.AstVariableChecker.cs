using FluentAssertions;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    partial class AstVisitorTests
    {
        public class AstVariableChecker : AstVisitor
        {
            public override void VisitAssignment(AstAssignment assign)
            {
                assign.HasVariable.Should().BeTrue();
                assign.VisitChildren(this);
            }
            public override void VisitExpressionOperand(AstExpressionOperand operand)
            {
                if (!operand.HasExpression &&
                    operand.LiteralNumeric is null &&
                    operand.FunctionReference is null)
                {
                    operand.VariableReference.Should().NotBeNull();
                }
                operand.VisitChildren(this);
            }
            public override void VisitVariableReference(AstVariableReference variable)
            {
                variable.VisitChildren(this);
            }
        }
    }
}
