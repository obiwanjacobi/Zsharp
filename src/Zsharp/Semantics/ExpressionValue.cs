using System;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public static class ExpressionExtensions
    {
        public static UInt64 ConstantValue(this AstExpression expression)
        {
            var visitor = new ExpressionValue();
            expression.Accept(visitor);
            return visitor.Value;
        }
    }

    public class ExpressionValue : AstVisitor
    {
        private readonly Stack<UInt64> _values = new Stack<UInt64>();

        public UInt64 Value
        {
            get { return _values.Peek(); }
        }

        public override void VisitExpression(AstExpression expression)
        {
            VisitChildren(expression);

            UInt64 lhs = 0;
            if (expression.LHS != null)
            {
                lhs = _values.Pop();
            }
            if (expression.RHS == null)
            {
                throw new InvalidOperationException("Invalid Expression: No Right-Hand-Side.");
            }

            UInt64 rhs = _values.Pop();

            _values.Push(PerformOperation(lhs, expression.Operator, rhs));
        }

        private ulong PerformOperation(ulong lhs, AstExpressionOperator op, ulong rhs)
        {
            return op switch
            {
                AstExpressionOperator.Number => rhs,
                AstExpressionOperator.Plus => lhs + rhs,
                AstExpressionOperator.Minus => lhs - rhs,
                AstExpressionOperator.Multiply => lhs * rhs,
                AstExpressionOperator.Divide => lhs / rhs,
                AstExpressionOperator.Modulo => lhs % rhs,
                AstExpressionOperator.Power => lhs ^ rhs,
                _ => throw new NotImplementedException($"Operator {op} not implemented.")
            };
        }

        public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            _values.Push(literalBool.Value ? 1 : 0);
        }

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            _values.Push(numeric.Value);
        }

        public override void VisitLiteralString(AstLiteralString literalString)
        {
            throw new NotImplementedException();
        }
    }
}
