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

        protected EmitContext EmitContext => _context;

        public override void VisitExpression(AstExpression expression)
        {
            if (expression.IsOperator(AstExpressionOperator.MaskArithmetic))
            {
                VisitChildren(expression);
                EmitArithmeticOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskBitwise))
            {
                VisitChildren(expression);
                EmitBitwiseOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskComparison))
            {
                VisitChildren(expression);
                EmitComparisonOperation(expression);
            }
            else if (expression.IsOperator(AstExpressionOperator.MaskLogic))
            {
                EmitLogicOperation(expression);
            }
            else
            {
                VisitChildren(expression);
            }
        }

        private void EmitArithmeticOperation(AstExpression expression)
        {
            var errTxt = $"Arithmetic Expression Operator {expression.Operator} is not implemented yet.";
            var il = EmitContext.InstructionFactory;

            bool isUnsigned = expression.TypeReference.TypeDefinition.IsUnsigned;

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.Plus => il.ArithmeticAdd(isUnsigned),
                AstExpressionOperator.Minus => il.ArithmeticSubtract(isUnsigned),
                AstExpressionOperator.Divide => il.ArithmeticDivide(isUnsigned),
                AstExpressionOperator.Multiply => il.ArithmeticMultiple(isUnsigned),
                AstExpressionOperator.Modulo => il.ArithmeticModulo(isUnsigned),
                AstExpressionOperator.Power => throw new NotImplementedException(errTxt),
                AstExpressionOperator.Negate => il.ArithmeticNegate(),
                _ => throw new InvalidOperationException(
                    $"Unrecognized Arithmetic Expression Operator {expression.Operator}"),
            };

            EmitContext.CodeBuilder.CodeBlock.Add(instruction);
        }

        private void EmitBitwiseOperation(AstExpression expression)
        {
            var errTxt = $"Bitwise Expression Operator {expression.Operator} is not implemented yet.";
            var il = EmitContext.InstructionFactory;

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.BitAnd => il.BitwiseAnd(),
                AstExpressionOperator.BitOr => il.BitwiseOr(),
                AstExpressionOperator.BitXor => il.BitwiseXor(),
                AstExpressionOperator.BitNegate => throw new NotImplementedException(errTxt),
                _ => null,
            };

            if (instruction != null)
            {
                EmitContext.CodeBuilder.CodeBlock.Add(instruction);
                return;
            }

            var instructions = expression.Operator switch
            {
                AstExpressionOperator.BitShiftLeft => il.BitwiseShiftLeft(),
                AstExpressionOperator.BitShiftRight => il.BitwiseShiftRight(),
                AstExpressionOperator.BitRollLeft => throw new NotImplementedException(errTxt),
                AstExpressionOperator.BitRollRight => throw new NotImplementedException(errTxt),
                _ => throw new InvalidOperationException(
                    $"Unrecognized Bitwise Expression Operator {expression.Operator}"),
            };

            EmitContext.CodeBuilder.CodeBlock.Add(instructions);
        }

        private void EmitComparisonOperation(AstExpression expression)
        {
            var il = _context.InstructionFactory;

            var instruction = expression.Operator switch
            {
                AstExpressionOperator.Equal => il.CompareEqual(),
                AstExpressionOperator.Greater => il.CompareGreater(),
                AstExpressionOperator.Smaller => il.CompareLesser(),
                _ => null
            };

            if (instruction != null)
            {
                EmitContext.CodeBuilder.CodeBlock.Add(instruction);
                return;
            }

            var instructions = expression.Operator switch
            {
                AstExpressionOperator.NotEqual => il.CompareNotEqual(),
                AstExpressionOperator.GreaterEqual => il.CompareGreaterEqual(),
                AstExpressionOperator.SmallerEqual => il.CompareLesserEqual(),
                _ => throw new InvalidOperationException(
                        $"Unrecognized Comparison Expression Operator {expression.Operator}")
            };

            EmitContext.CodeBuilder.CodeBlock.Add(instructions);
        }

        private void EmitLogicOperation(AstExpression expression)
        {
            /*
             * Logic operator implementation contain branches.
             * Also requires logic in-between and after emitting left and right hand sides.
             * 
             * `lhs && rhs`
             * 
             * <lhs>
             * br-ne not-true
             * 
             * <rhs>
             * br test
             * 
             * not-true:
             * ld 0
             * 
             * test:
             * br-ne ...
             * ...
             */

            var errTxt = $"Logic Expression Operator {expression.Operator} is not implemented yet.";

            // TODO: build up code around lhs/rhs
            expression.LHS?.Accept(this);
            expression.RHS?.Accept(this);

            throw expression.Operator switch
            {
                AstExpressionOperator.And => new NotImplementedException(errTxt),
                AstExpressionOperator.Or => new NotImplementedException(errTxt),
                AstExpressionOperator.Not => new NotImplementedException(errTxt),
                _ => new InvalidOperationException(
                    $"Unrecognized Logic Expression Operator {expression.Operator}"),
            };
        }

        public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            var il = EmitContext.InstructionFactory;
            EmitContext.CodeBuilder.CodeBlock.Add(
                il.LoadConstant(literalBool.Value));
        }

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            var il = EmitContext.InstructionFactory;
            EmitContext.CodeBuilder.CodeBlock.Add(
                il.LoadConstant(numeric.Value, numeric.GetBitCount()));
        }

        public override void VisitLiteralString(AstLiteralString literalString)
        {
            var il = EmitContext.InstructionFactory;
            EmitContext.CodeBuilder.CodeBlock.Add(
                il.LoadConstant(literalString.Value));
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var name = variable.Identifier.CanonicalName;
            var il = EmitContext.InstructionFactory;

            if (variable.VariableDefinition != null)
            {
                if (variable.IsTopLevel())
                {
                    var field = EmitContext.ModuleClass.GetField(name);
                    EmitContext.CodeBuilder.CodeBlock.Add(
                        il.LoadField(field));
                }
                else
                {
                    var varDef = EmitContext.CodeBuilder.GetVariable(name);
                    EmitContext.CodeBuilder.CodeBlock.Add(
                        il.LoadVariable(varDef));
                }
            }
            if (variable.ParameterDefinition != null)
            {
                var paramDef = EmitContext.CodeBuilder.GetParameter(name);
                EmitContext.CodeBuilder.CodeBlock.Add(
                    il.LoadParameter(paramDef));
            }
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            var functionDef = function.FunctionDefinition;
            if (functionDef.IsIntrinsic)
            {
                var intrinsic = new EmitIntrinsic(EmitContext);
                intrinsic.EmitFunction(function, (AstFunctionDefinitionIntrinsic)functionDef);
            }
            else
            {
                VisitChildren(function);

                var method = EmitContext.GetFunctionReference(functionDef);
                EmitContext.CodeBuilder.CodeBlock.Add(
                    EmitContext.InstructionFactory.Call(method));
            }
        }
    }
}
