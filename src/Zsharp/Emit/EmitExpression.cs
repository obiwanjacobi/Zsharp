using Mono.Cecil.Cil;
using System;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public class EmitExpression : AstVisitor
    {
        private readonly EmitContext _context;

        public EmitExpression(EmitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void VisitExpression(AstExpression expression)
        {
            base.VisitExpression(expression);
            var il = _context.ILProcessor;
            var errTxt = $"Expression Operator: {expression.Operator} is not implemented yet.";

            switch (expression.Operator)
            {
                case AstExpressionOperator.Plus:
                    il.Append(il.Create(OpCodes.Add));
                    break;
                case AstExpressionOperator.Minus:
                    il.Append(il.Create(OpCodes.Sub));
                    break;
                case AstExpressionOperator.Divide:
                    il.Append(il.Create(OpCodes.Div));
                    break;
                case AstExpressionOperator.Multiply:
                    il.Append(il.Create(OpCodes.Mul));
                    break;
                case AstExpressionOperator.Modulo:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Power:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Negate:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Equal:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.NotEqual:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Greater:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Smaller:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.GreaterEqual:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.SmallerEqual:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.BitAnd:
                    il.Append(il.Create(OpCodes.And));
                    break;
                case AstExpressionOperator.BitOr:
                    il.Append(il.Create(OpCodes.Or));
                    break;
                case AstExpressionOperator.BitXor:
                    il.Append(il.Create(OpCodes.Xor));
                    break;
                case AstExpressionOperator.BitShiftLeft:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.BitShiftRight:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.BitRollLeft:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.BitRollRight:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.BitNegate:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.And:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Or:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Not:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Number:
                    // handled by VisitNumeric
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Cannot generate code for Expression Operator: {expression.Operator}.");
            }
        }

        public override void VisitNumeric(AstNumeric numeric)
        {
            var il = _context.ILProcessor;
            il.Append(il.Create(OpCodes.Ldc_I4, (Int32)numeric.AsSigned()));
        }
    }
}
