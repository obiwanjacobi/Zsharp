using System;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.Eval;

internal static class EvaluateConstant
{
    public static IrConstant? Evaluate(TypeSymbol leftType, IrConstant leftValue,
        IrOperatorBinary op, TypeSymbol rightType, IrConstant rightValue)
    {
        if (!IrTypeConversion.TryDecideType(leftType, rightType, out var type))
            return null;

        object? val = null;

        if (TypeSymbol.IsBoolean(type))
        {
            var leftBool = leftValue.ToBool();
            var rightBool = rightValue.ToBool();

            val = op.Kind switch
            {
                IrOperatorBinaryKind.And => leftBool && rightBool,
                IrOperatorBinaryKind.Equals => leftBool == rightBool,
                IrOperatorBinaryKind.NotEquals => leftBool != rightBool,
                IrOperatorBinaryKind.Or => leftBool || rightBool,
                IrOperatorBinaryKind.Xor => leftBool ^ rightBool,
                _ => null
            };
        }

        if (TypeSymbol.IsInteger(type))
        {
            var leftInt = leftValue.ToI64();
            var rightInt = rightValue.ToI64();

            val = op.Kind switch
            {
                IrOperatorBinaryKind.Add => leftInt + rightInt,
                IrOperatorBinaryKind.BitwiseAnd => leftInt & rightInt,
                IrOperatorBinaryKind.BitwiseOr => leftInt | rightInt,
                IrOperatorBinaryKind.BitwiseRollLeft => leftInt << (int)rightInt,
                IrOperatorBinaryKind.BitwiseRollRight => leftInt >> (int)rightInt,
                IrOperatorBinaryKind.BitwiseShiftLeft => leftInt << (int)rightInt,
                IrOperatorBinaryKind.BitwiseShiftRight => leftInt >> (int)rightInt,
                IrOperatorBinaryKind.BitwiseShiftRightSign => leftInt >> (int)rightInt,
                IrOperatorBinaryKind.BitwiseXor => leftInt ^ rightInt,
                IrOperatorBinaryKind.Divide => leftInt / rightInt,
                IrOperatorBinaryKind.Equals => leftInt == rightInt,
                IrOperatorBinaryKind.Greater => leftInt > rightInt,
                IrOperatorBinaryKind.GreaterOrEquals => leftInt >= rightInt,
                IrOperatorBinaryKind.Lesser => leftInt < rightInt,
                IrOperatorBinaryKind.LesserOrEquals => leftInt <= rightInt,
                IrOperatorBinaryKind.Modulo => leftInt % rightInt,
                IrOperatorBinaryKind.Multiply => leftInt * rightInt,
                IrOperatorBinaryKind.NotEquals => leftInt != rightInt,
                IrOperatorBinaryKind.Power => (long)Math.Pow(leftInt, rightInt),
                IrOperatorBinaryKind.Root => (long)Math.Log(leftInt, rightInt),
                IrOperatorBinaryKind.Subtract => leftInt - rightInt,
                _ => null
            };
        }

        if (TypeSymbol.IsFloat(type))
        {
            var leftFloat = leftValue.ToF64();
            var rightFloat = rightValue.ToF64();

            val = op.Kind switch
            {
                IrOperatorBinaryKind.Add => leftFloat + rightFloat,
                IrOperatorBinaryKind.Divide => leftFloat / rightFloat,
                IrOperatorBinaryKind.Equals => leftFloat == rightFloat,
                IrOperatorBinaryKind.Greater => leftFloat > rightFloat,
                IrOperatorBinaryKind.GreaterOrEquals => leftFloat >= rightFloat,
                IrOperatorBinaryKind.Lesser => leftFloat < rightFloat,
                IrOperatorBinaryKind.LesserOrEquals => leftFloat <= rightFloat,
                IrOperatorBinaryKind.Modulo => leftFloat % rightFloat,
                IrOperatorBinaryKind.Multiply => leftFloat * rightFloat,
                IrOperatorBinaryKind.NotEquals => leftFloat != rightFloat,
                IrOperatorBinaryKind.Power => Math.Pow(leftFloat, rightFloat),
                IrOperatorBinaryKind.Root => Math.Log(leftFloat, rightFloat),
                IrOperatorBinaryKind.Subtract => leftFloat - rightFloat,
                _ => null
            };
        }

        return val is null ? null : new IrConstant(val);
    }
}
