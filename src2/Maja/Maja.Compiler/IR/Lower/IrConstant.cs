namespace Maja.Compiler.IR.Lower;

// compile time constant
internal class IrConstant
{
    internal IrConstant(object value)
    {
        Value = value;
    }

    public object Value { get; }

    public static IrConstant Zero => new(0);

    public static IrConstant? Fold(IrExpression left, IrBinaryOperator op, IrExpression right)
    {
        if (left.ConstantValue is null || right.ConstantValue is null)
            return null;

        return Evaluator.Evaluate(left.ConstantValue, op, right.ConstantValue);
    }
}
