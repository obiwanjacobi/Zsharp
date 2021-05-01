using System;

namespace Zsharp.AST
{
    public static class AstExpressionExtensions
    {
        public static string AsString(this AstExpressionOperator op)
        {
            return op switch
            {
                AstExpressionOperator.Open => "(",
                AstExpressionOperator.Close => ")",
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

        public static string AsString(this AstLiteralNumeric numeric)
        {
            return numeric.Value.ToString();
        }

        public static string AsString(this AstExpression expression)
        {
            if (expression == null)
            { return String.Empty; }

            var lhs = expression.LHS;
            var rhs = expression.RHS;

            return AsString(lhs) + AsString(expression.Operator) + AsString(rhs);
        }

        public static string AsString(this AstExpressionOperand? operand)
        {
            if (operand == null)
            { return String.Empty; }

            var expr = operand.Expression;
            if (expr != null)
            { return AsString(expr); }

            var num = operand.LiteralNumeric;
            if (num != null)
            { return AsString(num); }

            var bl = operand.LiteralBoolean;
            if (bl != null)
            { return bl.Value.ToString(); }

            var str = operand.LiteralString;
            if (str != null)
            { return str.Value; }

            var varRef = operand.VariableReference;
            if (varRef != null)
            { return AsString(varRef); }

            var funRef = operand.FunctionReference;
            if (funRef != null)
            { return funRef.AsString(); }

            return String.Empty;
        }

        private static string AsString(this IAstIdentifierSite? identifierSite)
        {
            return identifierSite?.Identifier?.Name ?? String.Empty;
        }
    }
}
