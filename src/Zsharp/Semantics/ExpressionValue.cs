using System;
using System.Collections.Generic;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public static class ExpressionExtensions
    {
        public static Int64 ConstantValue(this AstExpression expression)
        {
            var visitor = new ExpressionValue();
            expression.Accept(visitor);
            return visitor.ValueInt.GetValueOrDefault();
        }
    }

    public class ExpressionValue : AstVisitor
    {
        private readonly Stack<object> _values = new();

        private Int64 PopInt()
            => (Int64)_values.Pop();

        private bool PopBool()
            => (bool)_values.Pop();

        public bool HasValue => _values.Count == 1;

        public Int64? ValueInt
        {
            get
            {
                var val = _values.Peek();
                return val is Int64 ? (Int64)val : (Int64?)null;
            }
        }

        public bool? ValueBool
        {
            get
            {
                var val = _values.Peek();
                return val is bool ? (bool)val : (bool?)null;
            }
        }

        public override void VisitExpression(AstExpression expression)
        {
            expression.VisitChildren(this);

            Ast.Guard(expression.RHS, "Invalid Expression: No Right-Hand-Side.");

            object value;
            if ((expression.Operator & AstExpressionOperator.MaskComparison) > 0)
            {
                Int64 rhs = PopInt();
                Int64 lhs = expression.LHS is not null ? PopInt() : 0;

                value = PerformComparisonOperation(lhs, expression.Operator, rhs);
            }
            else if ((expression.Operator & AstExpressionOperator.MaskLogic) > 0)
            {
                bool rhs = PopBool();
                bool lhs = expression.LHS is not null && PopBool();

                value = PerformLogicalOperation(lhs, expression.Operator, rhs);
            }
            else // arithmetic and bitwise
            {
                Int64 rhs = PopInt();
                Int64 lhs = expression.HasLHS ? PopInt() : 0;

                value = PerformNumericalOperation(lhs, expression.Operator, rhs);
            }

            _values.Push(value);
        }

        private Int64 PerformNumericalOperation(Int64 lhs, AstExpressionOperator op, Int64 rhs)
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
                AstExpressionOperator.Negate => -rhs,

                AstExpressionOperator.BitAnd => lhs & rhs,
                AstExpressionOperator.BitOr => lhs | rhs,
                AstExpressionOperator.BitNegate => ~rhs,
                AstExpressionOperator.BitXor => lhs ^ rhs,
                AstExpressionOperator.BitShiftLeft => lhs << (int)rhs,
                AstExpressionOperator.BitShiftRight => lhs >> (int)rhs,

                _ => throw new InvalidOperationException($"Invalid Arithmetic operator: {op}.")
            };
        }

        private bool PerformComparisonOperation(Int64 lhs, AstExpressionOperator op, Int64 rhs)
        {
            return op switch
            {
                AstExpressionOperator.Equal => lhs == rhs,
                AstExpressionOperator.NotEqual => lhs != rhs,
                AstExpressionOperator.GreaterEqual => lhs >= rhs,
                AstExpressionOperator.SmallerEqual => lhs <= rhs,
                AstExpressionOperator.Greater => lhs > rhs,
                AstExpressionOperator.Smaller => lhs < rhs,

                _ => throw new InvalidOperationException($"Invalid Comparison operator: {op}.")
            };
        }

        private bool PerformLogicalOperation(bool lhs, AstExpressionOperator op, bool rhs)
        {
            return op switch
            {
                AstExpressionOperator.And => lhs && rhs,
                AstExpressionOperator.Or => lhs | rhs,
                AstExpressionOperator.Not => !rhs,

                _ => throw new InvalidOperationException($"Invalid Logical operator: {op}.")
            };
        }

        public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            _values.Push(literalBool.Value ? 1 : 0);
        }

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            _values.Push((Int64)numeric.Value);
        }

        public override void VisitLiteralString(AstLiteralString literalString)
        {
            throw new NotImplementedException();
        }
    }
}
