using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal static class IrNumber
{
    internal static bool TryDecideType(TypeSymbol first, TypeSymbol second,
        [NotNullWhen(true)] out TypeSymbol? type)
    {
        if (first is TypeInferredSymbol inferredFirst)
            first = inferredFirst.GetPreferredType()
                ?? throw new Exception("No preferred type could be found.");
        if (second is TypeInferredSymbol inferredSecond)
            second = inferredSecond.GetPreferredType()
                ?? throw new Exception("No preferred type could be found.");

        if ((IsInteger(first) && IsInteger(second)) ||
            (IsFloat(first) && IsFloat(second)))
        {
            type = first.SizeInBytes < second.SizeInBytes
                ? second
                : first
                ;
            return true;
        }

        if (IsBoolean(first) && IsBoolean(second))
        {
            type = first;
            return true;
        }

        type = null;
        return false;
    }

    internal static bool IsBoolean(TypeSymbol type)
        => type == TypeSymbol.Bool;

    internal static bool IsInteger(TypeSymbol type)
    {
        return type == TypeSymbol.I8
            || type == TypeSymbol.U8
            || type == TypeSymbol.I16
            || type == TypeSymbol.U16
            || type == TypeSymbol.I32
            || type == TypeSymbol.U32
            || type == TypeSymbol.I64
            || type == TypeSymbol.I64
            ;
    }

    internal static bool IsFloat(TypeSymbol type)
    {
        return type == TypeSymbol.F16
            || type == TypeSymbol.F32
            || type == TypeSymbol.F64
            || type == TypeSymbol.F96
            ;
    }

    internal static IEnumerable<TypeSymbol> ParseNumber(string text, out object? value)
    {
        value = null;
        var types = new List<TypeSymbol>();

        // signed
        if (SByte.TryParse(text, out var i8))
        {
            value ??= i8;
            types.Add(TypeSymbol.I8);
        }
        if (Int16.TryParse(text, out var i16))
        {
            value ??= i16;
            types.Add(TypeSymbol.I16);
        }
        if (Int32.TryParse(text, out var i32))
        {
            value ??= i32;
            types.Add(TypeSymbol.I32);
        }
        if (Int64.TryParse(text, out var i64))
        {
            value ??= i64;
            types.Add(TypeSymbol.I64);
        }

        // floating point
        if (Single.TryParse(text, out var f32))
        {
            value ??= f32;
            types.Add(TypeSymbol.F32);
        }
        if (Double.TryParse(text, out var f64))
        {
            value ??= f64;
            types.Add(TypeSymbol.F64);
        }
        if (Decimal.TryParse(text, out var f96))
        {
            value ??= f96;
            types.Add(TypeSymbol.F96);
        }

        // unsigned
        if (Byte.TryParse(text, out var u8))
        {
            types.Add(TypeSymbol.U8);
        }
        if (UInt16.TryParse(text, out var u16))
        {
            types.Add(TypeSymbol.U16);
        }
        if (UInt32.TryParse(text, out var u32))
        {
            types.Add(TypeSymbol.U32);
        }
        if (UInt64.TryParse(text, out var u64))
        {
            types.Add(TypeSymbol.U64);
        }

        //if (types.Count == 0)
        //    throw new NotSupportedException($"IR: No support for parsing number: '{text}'.");

        return types;
    }
}