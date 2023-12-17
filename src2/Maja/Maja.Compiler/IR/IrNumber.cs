using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal static class IrNumber
{
    internal static List<TypeSymbol> ParseNumber(string text, out object? value)
    {
        value = null;
        var types = new List<TypeSymbol>();

        // signed/unsigned
        if (SByte.TryParse(text, out var i8))
        {
            value = i8;
            types.Add(TypeSymbol.I8);
        }
        if (Byte.TryParse(text, out var u8))
        {
            value ??= u8;
            types.Add(TypeSymbol.U8);
        }
        if (Int16.TryParse(text, out var i16))
        {
            value ??= i16;
            types.Add(TypeSymbol.I16);
        }
        if (UInt16.TryParse(text, out var u16))
        {
            value ??= u16;
            types.Add(TypeSymbol.U16);
        }
        if (Int32.TryParse(text, out var i32))
        {
            value ??= i32;
            types.Add(TypeSymbol.I32);
        }
        if (UInt32.TryParse(text, out var u32))
        {
            value ??= u32;
            types.Add(TypeSymbol.U32);
        }
        if (Int64.TryParse(text, out var i64))
        {
            value ??= i64;
            types.Add(TypeSymbol.I64);
        }
        if (UInt64.TryParse(text, out var u64))
        {
            value ??= u64;
            types.Add(TypeSymbol.U64);
        }

        // floating point
        if (Half.TryParse(text, CultureInfo.InvariantCulture, out var f16))
        {
            value ??= f16;
            types.Add(TypeSymbol.F16);
        }
        if (Single.TryParse(text, CultureInfo.InvariantCulture, out var f32))
        {
            value ??= f32;
            if (value is Half hval && (float)hval < f32) value = f32;
            types.Add(TypeSymbol.F32);
        }
        if (Double.TryParse(text, CultureInfo.InvariantCulture, out var f64))
        {
            value ??= f64;
            if (value is float fval && (double)fval < f64) value = f64;
            types.Add(TypeSymbol.F64);
        }
        if (Decimal.TryParse(text, CultureInfo.InvariantCulture, out var f96))
        {
            value ??= f96;
            if (value is double dval && (decimal)dval < f96) value = f96;
            types.Add(TypeSymbol.F96);
        }

        return types;
    }

    public static int SizeInBytes(this TypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsWellknown)
            return 0;

        return typeSymbol.Name.Value[1..] switch
        {
            "oid" => 0,
            "ool" => 1,
            "8" => 1,
            "16" => 2,
            "32" => 4,
            "64" => 8,
            "96" => 12,
            _ => 0
        };
    }
}