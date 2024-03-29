﻿using System;

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
            if (expression is null)
            { return String.Empty; }

            var lhs = expression.HasLHS ? expression.LHS : null;
            var rhs = expression.HasRHS ? expression.RHS : null;

            return AsString(lhs) + AsString(expression.Operator) + AsString(rhs);
        }

        public static string AsString(this AstExpressionOperand? operand)
        {
            if (operand is null)
            { return String.Empty; }
            
            if (operand.HasExpression)
            {
                var expr = operand.Expression;
                return AsString(expr);
            }

            var num = operand.LiteralNumeric;
            if (num is not null)
            { return AsString(num); }

            var bl = operand.LiteralBoolean;
            if (bl is not null)
            { return bl.Value.ToString(); }

            var str = operand.LiteralString;
            if (str is not null)
            { return str.Value; }

            var varRef = operand.VariableReference;
            if (varRef is not null)
            { return AsString(varRef); }

            var funRef = operand.FunctionReference;
            if (funRef is not null)
            { return funRef.AsString(); }

            return String.Empty;
        }

        public static string AsString(this IAstIdentifierSite? identifierSite)
            => identifierSite?.Identifier.NativeFullName ?? String.Empty;
    }
}
