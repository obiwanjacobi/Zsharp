using System;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class EmitExpression : AstVisitor
    {
        private readonly EmitContext _context;

        public EmitExpression(EmitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected EmitContext EmitContext => _context;

        public override void VisitExpression(AstExpression expression)
        {
            expression.LHS?.Accept(this);

            if (expression.IsOperator(AstExpressionOperator.MaskArithmetic))
            {
                EmitArithmeticOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskBitwise))
            {
                EmitBitwiseOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskComparison))
            {
                EmitComparisonOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskLogic))
            {
                EmitLogicOperation(expression);
            }
            //else
            //{
            //    VisitChildren(expression);
            //}

            expression.RHS?.Accept(this);
        }

        private void EmitArithmeticOperation(AstExpression expression)
        {
            var errTxt = $"Arithmetic Expression Operator {expression.Operator} is not implemented yet.";

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.Plus => "+",
                AstExpressionOperator.Minus => "-",
                AstExpressionOperator.Divide => "/",
                AstExpressionOperator.Multiply => "*",
                AstExpressionOperator.Modulo => "%",
                AstExpressionOperator.Power => throw new NotImplementedException(errTxt),
                AstExpressionOperator.Negate => "-",
                _ => throw new ZsharpException(
                    $"Unrecognized Arithmetic Expression Operator {expression.Operator}"),
            };

            EmitContext.CsBuilder.Append(instruction);
        }

        private void EmitBitwiseOperation(AstExpression expression)
        {
            var errTxt = $"Bitwise Expression Operator {expression.Operator} is not implemented yet.";

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.BitAnd => "&",
                AstExpressionOperator.BitOr => "|",
                AstExpressionOperator.BitXor => "^",
                AstExpressionOperator.BitNegate => "~",
                AstExpressionOperator.BitShiftLeft => "<<",
                AstExpressionOperator.BitShiftRight => ">>",
                AstExpressionOperator.BitRollLeft => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitRollRight => throw new NotImplementedException(errTxt),
                _ => throw new ZsharpException(
                    $"Unrecognized Bitwise Expression Operator {expression.Operator}"),
            };

            EmitContext.CsBuilder.Append(instruction);
        }

        private void EmitComparisonOperation(AstExpression expression)
        {
            var instruction = expression.Operator switch
            {
                AstExpressionOperator.Equal => "==",
                AstExpressionOperator.Greater => ">",
                AstExpressionOperator.Smaller => "<",
                AstExpressionOperator.NotEqual => "!=",
                AstExpressionOperator.GreaterEqual => ">=",
                AstExpressionOperator.SmallerEqual => "<=",
                _ => throw new ZsharpException(
                        $"Unrecognized Comparison Expression Operator {expression.Operator}")
            };

            EmitContext.CsBuilder.Append(instruction);
        }

        private void EmitLogicOperation(AstExpression expression)
        {
            var instruction = expression.Operator switch
            {
                AstExpressionOperator.And => "&&",
                AstExpressionOperator.Or => "||",
                AstExpressionOperator.Not => "!",
                _ => throw new ZsharpException(
                    $"Unrecognized Logic Expression Operator {expression.Operator}"),
            };

            EmitContext.CsBuilder.Append(instruction);
        }

        public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            EmitContext.CsBuilder.Append(literalBool.Value.ToString());
        }

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            EmitContext.CsBuilder.Append(numeric.Value.ToString());
        }

        public override void VisitLiteralString(AstLiteralString literalString)
        {
            EmitContext.CsBuilder.Append($"\"{literalString.Value}\"");
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var name = variable.Identifier.CanonicalName;
            EmitContext.CsBuilder.Append(name);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            EmitContext.CsBuilder.WriteIndent();
            EmitContext.CsBuilder.Append($"{function.Identifier.CanonicalName}(");

            VisitChildren(function);

            EmitContext.CsBuilder.Append(")");
        }
    }
}
