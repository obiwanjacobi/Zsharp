using System;
using Zsharp.AST;

namespace Zsharp.Dgml
{
    public static class AstExtensions
    {
        public static string AsString(this AstExpressionOperator op)
        {
            return op switch
            {
                AstExpressionOperator.Open => "(",
                AstExpressionOperator.Close => ")(",
                // arithmetic
                AstExpressionOperator.Plus => "+",
                AstExpressionOperator.Minus => "-",
                AstExpressionOperator.Divide => "/",
                AstExpressionOperator.Multiply => "*",
                AstExpressionOperator.Modulo => "%",
                AstExpressionOperator.Power => "**",
                AstExpressionOperator.Negate => "-",
                // comparison
                AstExpressionOperator.Equal => "=",
                AstExpressionOperator.NotEqual => "<>",
                AstExpressionOperator.Greater => ">",
                AstExpressionOperator.Smaller => "<",
                AstExpressionOperator.GreaterEqual => ">=",
                AstExpressionOperator.SmallerEqual => "<=",
                // bitwise
                AstExpressionOperator.BitAnd => "&",
                AstExpressionOperator.BitOr => "|",
                AstExpressionOperator.BitXor => "^",
                AstExpressionOperator.BitShiftLeft => "<<",
                AstExpressionOperator.BitShiftRight => ">>",
                AstExpressionOperator.BitRollLeft => "|<",
                AstExpressionOperator.BitRollRight => ">|",
                AstExpressionOperator.BitNegate => "~",
                // logic
                AstExpressionOperator.And => "and",
                AstExpressionOperator.Or => "or",
                AstExpressionOperator.Not => "not",
                _ => String.Empty,
            };
        }

        public static string AsString(this AstNumeric numeric)
        {
            return numeric.Context.GetText();
        }

        public static string AsString(this AstExpression expression)
        {
            if (expression == null)
            { return String.Empty; }

            var lhs = expression.LHS;
            var rhs = expression.RHS;

            return AsString(lhs) + AsString(expression.Operator) + AsString(rhs);
        }

        public static string AsString(this AstExpressionOperand operand)
        {
            if (operand == null)
            { return String.Empty; }

            var expr = operand.Expression;
            if (expr != null)
            { return AsString(expr); }

            var num = operand.Numeric;
            if (num != null)
            { return AsString(num); }

            var context = operand.Context;
            if (context != null)
            { return context.GetText(); }

            return String.Empty;
        }
    }
}
