using System;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal static class IrNumber
{
    internal static void ParseNumber(string text, out object value, out TypeSymbol type)
    {
        if (SByte.TryParse(text, out var i8))
        {
            value = i8;
            type = TypeSymbol.I8;
            return;
        }
        if (Int16.TryParse(text, out var i16))
        {
            value = i16;
            type = TypeSymbol.I16;
            return;
        }

        if (Int32.TryParse(text, out var i32))
        {
            value = i32;
            type = TypeSymbol.I32;
            return;
        }

        if (Int64.TryParse(text, out var i64))
        {
            value = i64;
            type = TypeSymbol.I64;
            return;
        }

        if (Single.TryParse(text, out var f32))
        {
            value = f32;
            type = TypeSymbol.F32;
            return;
        }

        if (Double.TryParse(text, out var f64))
        {
            value = f64;
            type = TypeSymbol.F64;
            return;
        }

        if (Decimal.TryParse(text, out var f96))
        {
            value = f96;
            type = TypeSymbol.F96;
            return;
        }

        throw new NotSupportedException($"IR: No support for parsing number: '{text}'.");
    }
}