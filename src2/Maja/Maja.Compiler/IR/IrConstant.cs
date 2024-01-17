using System;
using System.Diagnostics;
using Maja.Compiler.Eval;

namespace Maja.Compiler.IR;

// compile time constant
[DebuggerDisplay("{DebuggerDisplay()}")]
internal class IrConstant
{
    internal IrConstant(object value)
    {
        Value = value;
    }

    public object Value { get; }

    public bool ToBool()
        => Convert.ToBoolean(Value);
    public sbyte ToI8()
        => Convert.ToSByte(Value);
    public byte ToU8()
        => Convert.ToByte(Value);
    public short ToI16()
        => Convert.ToInt16(Value);
    public ushort ToU16()
        => Convert.ToUInt16(Value);
    public int ToI32()
        => Convert.ToInt32(Value);
    public uint ToU32()
        => Convert.ToUInt32(Value);
    public long ToI64()
        => Convert.ToInt64(Value);
    public ulong ToU64()
        => Convert.ToUInt64(Value);
    public Half ToF16()
        => (Half)Value;
    public float ToF32()
        => Convert.ToSingle(Value);
    public double ToF64()
        => Convert.ToDouble(Value);
    public decimal ToF96()
        => Convert.ToDecimal(Value);

    public char ToC16()
        => Convert.ToChar(Value);
    public string ToStr()
        => Convert.ToString(Value) ?? string.Empty;

    internal string DebuggerDisplay()
        => ToString();
    public override string ToString()
        => Value?.ToString() ?? "<null>";
    public string AsString()
        => Value is string ? $"\"{Value}\"" : Value.ToString() ?? string.Empty;

    public static IrConstant Zero => new(0);

    public static IrConstant? Fold(IrExpression left, IrBinaryOperator op, IrExpression right)
    {
        if (left.ConstantValue is null || right.ConstantValue is null)
            return null;

        return EvaluateConstant.Evaluate(left.TypeSymbol,
            left.ConstantValue, op, right.TypeSymbol, right.ConstantValue);
    }
}
