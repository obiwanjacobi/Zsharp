using System;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class EmitExpression : AstVisitor
    {
        internal EmitExpression(CsBuilder builder)
        {
            _builder = builder;
        }

        private readonly CsBuilder _builder;
        internal CsBuilder CsBuilder => _builder;

        public override void VisitExpression(AstExpression expression)
        {
            if (expression.HasLHS)
                expression.LHS.Accept(this);

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
                _ => throw new InternalErrorException(
                    $"Unrecognized Arithmetic Expression Operator {expression.Operator}"),
            };

            AppendOperator(instruction);
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
                _ => throw new InternalErrorException(
                    $"Unrecognized Bitwise Expression Operator {expression.Operator}"),
            };

            AppendOperator(instruction);
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
                _ => throw new InternalErrorException(
                        $"Unrecognized Comparison Expression Operator {expression.Operator}")
            };

            AppendOperator(instruction);
        }

        private void EmitLogicOperation(AstExpression expression)
        {
            var instruction = expression.Operator switch
            {
                AstExpressionOperator.And => "&&",
                AstExpressionOperator.Or => "||",
                AstExpressionOperator.Not => "!",
                _ => throw new InternalErrorException(
                    $"Unrecognized Logic Expression Operator {expression.Operator}"),
            };

            AppendOperator(instruction);
        }

        public override void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            _builder.Append(literalBool.Value ? "true" : "false");
        }

        public override void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            _builder.Append(numeric.Value.ToString());
        }

        public override void VisitLiteralString(AstLiteralString literalString)
        {
            _builder.Append($"\"{literalString.Value}\"");
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var name = variable.Identifier!.SymbolName.CanonicalName.FullName;
            _builder.Append(name);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            var functionDef = function.FunctionDefinition!;
            var name = functionDef.Identifier.SymbolName.CanonicalName.FullName;

            if (functionDef.IsExternal)
            {
                name = ((AstFunctionDefinitionExternal)functionDef).ExternalName.FullName;
            }

            _builder.Append($"{name}(");

            function.VisitChildren(this);

            _builder.Append(")");
        }

        private void AppendOperator(string op)
        {
            _builder.Append(" ");
            _builder.Append(op);
            _builder.Append(" ");
        }
    }
}
