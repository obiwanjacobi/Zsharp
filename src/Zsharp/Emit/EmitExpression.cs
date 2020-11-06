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
        }

        private void EmitArithmeticOperation(AstExpression expression)
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

        private void EmitBitwiseOperation(AstExpression expression)
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

        private void EmitComparisonOperation(AstExpression expression)
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

        private void EmitLogicOperation(AstExpression expression)
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

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            var il = _context.InstructionFactory;
            var instruction = il.LoadConstant((Int32)numeric.AsSigned());
            _context.CodeBuilder.CodeBlock.Add(instruction);
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var name = variable.Identifier.Name;
            var il = _context.InstructionFactory;

            if (variable.VariableDefinition != null)
            {
                if (!_context.HasVariable(name))
                {
                    _context.AddVariable(variable.VariableDefinition);
                }
                var varDef = _context.CodeBuilder.GetVariable(name);
                var instruction = il.LoadVariable(varDef);
                _context.CodeBuilder.CodeBlock.Add(instruction);
            }
            if (variable.ParameterDefinition != null)
            {
                var paramDef = _context.CodeBuilder.GetParameter(name);
                var instruction = il.LoadParameter(paramDef);
                _context.CodeBuilder.CodeBlock.Add(instruction);
            }
        }
    }
}
