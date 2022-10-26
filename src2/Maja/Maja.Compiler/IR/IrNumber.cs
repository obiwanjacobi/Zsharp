using System;

namespace Maja.Compiler.IR;

internal class IrNumber
{
    internal static object ParseNumber(string text)
    {
        if (Int64.TryParse(text, out var i64))
            return i64;

        if (Double.TryParse(text, out var f64))
            return f64;

        if (Decimal.TryParse(text, out var f96))
            return f96;

        throw new NotSupportedException($"IR: No support for parsing number: '{text}'.");
    }
}