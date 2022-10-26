namespace Maja.Compiler.IR.Lower;

internal static class Evaluator
{
    public static IrConstant Evaluate(IrConstant left, IrBinaryOperator op, IrConstant right)
    {
        return IrConstant.Zero;
    }
}
