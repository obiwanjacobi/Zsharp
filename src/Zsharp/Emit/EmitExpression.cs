﻿using System;
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
            if (expression.IsOperator(AstExpressionOperator.MaskArithmetic))
            {
                EmitArithmeticExcperession(expression);
                return;
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskBitwise))
            {
                EmitBitwiseExpression(expression);
                return;
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskComparison))
            {
                EmitComparisonExpression(expression);
                return;
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskLogic))
            {
                EmitLogicExpression(expression);
                return;
            }

            base.VisitExpression(expression);
        }

        private void EmitArithmeticExcperession(AstExpression expression)
        {
            var errTxt = $"Arithmetic Expression Operator {expression.Operator} is not implemented yet.";
            var il = _context.InstructionFactory;

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.Plus => il.ArithmeticAdd(),
                AstExpressionOperator.Minus => il.ArithmeticSubtract(),
                AstExpressionOperator.Divide => il.ArithmeticDivide(),
                AstExpressionOperator.Multiply => il.ArithmeticMultiple(),
                AstExpressionOperator.Modulo => throw new NotImplementedException(errTxt),
                AstExpressionOperator.Power => throw new NotImplementedException(errTxt),
                AstExpressionOperator.Negate => throw new NotImplementedException(errTxt),
                _ => throw new InvalidOperationException(
                    $"Unrecognized Arithmetic Expression Operator {expression.Operator}"),
            };

            _context.CodeBuilder.CodeBlock.Add(instruction);
        }

        private void EmitBitwiseExpression(AstExpression expression)
        {
            var errTxt = $"Bitwise Expression Operator {expression.Operator} is not implemented yet.";
            var il = _context.InstructionFactory;

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.BitAnd => il.BitwiseAnd(),
                AstExpressionOperator.BitOr => il.BitwiseOr(),
                AstExpressionOperator.BitXor => il.BitwiseXor(),
                AstExpressionOperator.BitShiftLeft => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitShiftRight => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitRollLeft => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitRollRight => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitNegate => throw new NotImplementedException(errTxt),
                _ => throw new InvalidOperationException(
                    $"Unrecognized Bitwise Expression Operator {expression.Operator}"),
            };

            _context.CodeBuilder.CodeBlock.Add(instruction);
        }

        private void EmitComparisonExpression(AstExpression expression)
        {
            var errTxt = $"Comparison Expression Operator {expression.Operator} is not implemented yet.";

            switch (expression.Operator)
            {
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
                default:
                    throw new InvalidOperationException(
                        $"Unrecognized Comparison Expression Operator {expression.Operator}");
            }
        }

        private void EmitLogicExpression(AstExpression expression)
        {
            var errTxt = $"Logic Expression Operator {expression.Operator} is not implemented yet.";

            switch (expression.Operator)
            {
                case AstExpressionOperator.And:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Or:
                    throw new NotImplementedException(errTxt);
                case AstExpressionOperator.Not:
                    throw new NotImplementedException(errTxt);
                default:
                    throw new InvalidOperationException(
                        $"Unrecognized Logic Expression Operator {expression.Operator}");
            }
        }

        public override void VisitNumeric(AstNumeric numeric)
        {
            var il = _context.InstructionFactory;
            var instruction = il.LoadConstant((Int32)numeric.AsSigned());
            _context.CodeBuilder.CodeBlock.Add(instruction);
        }
    }
}