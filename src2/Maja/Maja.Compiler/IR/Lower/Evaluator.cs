using System;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR.Lower;

internal static class Evaluator
{
    public static IrConstant? Evaluate(TypeSymbol leftType, IrConstant leftValue,
        IrBinaryOperator op, TypeSymbol rightType, IrConstant rightValue)
    {
        if (!IrNumber.TryDecideType(leftType, rightType, out var type))
            return null;

        object? val = null;

        if (IrNumber.IsBoolean(type))
        {
            var leftBool = leftValue.ToBool();
            var rightBool = rightValue.ToBool();

            val = op.Kind switch
            {
                IrBinaryOperatorKind.And => leftBool && rightBool,
                IrBinaryOperatorKind.Equals => leftBool == rightBool,
                IrBinaryOperatorKind.NotEquals => leftBool != rightBool,
                IrBinaryOperatorKind.Or => leftBool || rightBool,
                IrBinaryOperatorKind.Xor => leftBool ^ rightBool,
                _ => null
            };
        }

        if (IrNumber.IsInteger(type))
        {
            var leftInt = leftValue.ToI64();
            var rightInt = rightValue.ToI64();

            val = op.Kind switch
            {
                IrBinaryOperatorKind.Add => leftInt + rightInt,
                IrBinaryOperatorKind.BitwiseAnd => leftInt & rightInt,
                IrBinaryOperatorKind.BitwiseOr => leftInt | rightInt,
                IrBinaryOperatorKind.BitwiseRollLeft => leftInt << (int)rightInt,
                IrBinaryOperatorKind.BitwiseRollRight => leftInt >> (int)rightInt,
                IrBinaryOperatorKind.BitwiseShiftLeft => leftInt << (int)rightInt,
                IrBinaryOperatorKind.BitwiseShiftRight => leftInt >> (int)rightInt,
                IrBinaryOperatorKind.BitwiseShiftRightSign => leftInt >> (int)rightInt,
                IrBinaryOperatorKind.BitwiseXor => leftInt ^ rightInt,
                IrBinaryOperatorKind.Divide => leftInt / rightInt,
                IrBinaryOperatorKind.Equals => leftInt == rightInt,
                IrBinaryOperatorKind.Greater => leftInt > rightInt,
                IrBinaryOperatorKind.GreaterOrEquals => leftInt >= rightInt,
                IrBinaryOperatorKind.Lesser => leftInt < rightInt,
                IrBinaryOperatorKind.LesserOrEquals => leftInt <= rightInt,
                IrBinaryOperatorKind.Modulo => leftInt % rightInt,
                IrBinaryOperatorKind.Multiply => leftInt * rightInt,
                IrBinaryOperatorKind.NotEquals => leftInt != rightInt,
                IrBinaryOperatorKind.Power => (long)Math.Pow(leftInt, rightInt),
                IrBinaryOperatorKind.Root => (long)Math.Log(leftInt, rightInt),
                IrBinaryOperatorKind.Subtract => leftInt - rightInt,
                _ => null
            };
        }

        if (IrNumber.IsFloat(type))
        {
            var leftFloat = leftValue.ToF64();
            var rightFloat = rightValue.ToF64();

            val = op.Kind switch
            {
                IrBinaryOperatorKind.Add => leftFloat + rightFloat,
                IrBinaryOperatorKind.Divide => leftFloat / rightFloat,
                IrBinaryOperatorKind.Equals => leftFloat == rightFloat,
                IrBinaryOperatorKind.Greater => leftFloat > rightFloat,
                IrBinaryOperatorKind.GreaterOrEquals => leftFloat >= rightFloat,
                IrBinaryOperatorKind.Lesser => leftFloat < rightFloat,
                IrBinaryOperatorKind.LesserOrEquals => leftFloat <= rightFloat,
                IrBinaryOperatorKind.Modulo => leftFloat % rightFloat,
                IrBinaryOperatorKind.Multiply => leftFloat * rightFloat,
                IrBinaryOperatorKind.NotEquals => leftFloat != rightFloat,
                IrBinaryOperatorKind.Power => Math.Pow(leftFloat, rightFloat),
                IrBinaryOperatorKind.Root => Math.Log(leftFloat, rightFloat),
                IrBinaryOperatorKind.Subtract => leftFloat - rightFloat,
                _ => null
            };
        }

        return val is null ? null : new IrConstant(val);
    }
}
