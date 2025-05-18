using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal static class IrTypeConversion
{
    public static bool TryDecideType(TypeSymbol first, TypeSymbol second, [NotNullWhen(true)] out TypeSymbol? type)
    {
        TypeSymbol? newFirst = null;
        TypeSymbol? newSecond = null;

        if (first is TypeInferredSymbol inferredFirst)
        {
            if (!inferredFirst.TryGetPreferredType(out newFirst))
                throw new MajaException("No preferred type could be found for first.");
        }
        else
            newFirst = first;

        if (second is TypeInferredSymbol inferredSecond)
        {
            if (!inferredSecond.TryGetPreferredType(out newSecond))
                throw new MajaException("No preferred type could be found for second.");
        }
        else
            newSecond = second;

        if ((TypeSymbol.IsInteger(newFirst) && TypeSymbol.IsInteger(newSecond)) ||
            (TypeSymbol.IsFloat(newFirst) && TypeSymbol.IsFloat(newSecond)))
        {
            type = newFirst.SizeInBytes() < newSecond.SizeInBytes()
                ? newSecond
                : newFirst
                ;
            return true;
        }

        if (TypeSymbol.IsBoolean(newFirst) && TypeSymbol.IsBoolean(newSecond))
        {
            type = newFirst;
            return true;
        }

        type = null;
        return false;
    }

    public static bool TrySelectInferredType(this TypeInferredSymbol inferredSymbol, TypeSymbol fromType, [NotNullWhen(true)] out TypeSymbol? convertedType)
    {
        if (inferredSymbol.Candidates.Contains(fromType))
        {
            convertedType = fromType;
            return true;
        }

        // TODO: match candidates to fromType as best as possible.

        return inferredSymbol.TryGetPreferredType(out convertedType);
    }

    public static bool TryGetPreferredType(this TypeInferredSymbol inferredSymbol, [NotNullWhen(true)] out TypeSymbol? preferredType)
    {
        preferredType = null;

        if (inferredSymbol.Candidates.Contains(TypeSymbol.I64))
            preferredType = TypeSymbol.I64;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.I32))
            preferredType = TypeSymbol.I32;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.U64))
            preferredType = TypeSymbol.U64;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.U32))
            preferredType = TypeSymbol.U32;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.I16))
            preferredType = TypeSymbol.I16;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.U16))
            preferredType = TypeSymbol.U16;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.F64))
            preferredType = TypeSymbol.F64;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.F32))
            preferredType = TypeSymbol.F32;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.F96))
            preferredType = TypeSymbol.F96;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.I8))
            preferredType = TypeSymbol.I8;
        else if (inferredSymbol.Candidates.Contains(TypeSymbol.U8))
            preferredType = TypeSymbol.U8;

        return preferredType is not null;
    }

    public static bool TryGetNextBiggerType(TypeSymbol fromType, [NotNullWhen(true)] out TypeSymbol? nextBiggerType)
    {
        nextBiggerType = null;

        if (fromType == TypeSymbol.F16)
            nextBiggerType = TypeSymbol.F32;
        if (fromType == TypeSymbol.F32)
            nextBiggerType = TypeSymbol.F64;
        if (fromType == TypeSymbol.F64)
            nextBiggerType = TypeSymbol.F96;
        if (fromType == TypeSymbol.F96)
            nextBiggerType = TypeSymbol.F96;

        if (fromType == TypeSymbol.I8)
            nextBiggerType = TypeSymbol.I16;
        if (fromType == TypeSymbol.I16)
            nextBiggerType = TypeSymbol.I32;
        if (fromType == TypeSymbol.I32)
            nextBiggerType = TypeSymbol.I64;
        if (fromType == TypeSymbol.I64)
            nextBiggerType = TypeSymbol.I64;

        if (fromType == TypeSymbol.U8)
            nextBiggerType = TypeSymbol.U16;
        if (fromType == TypeSymbol.U16)
            nextBiggerType = TypeSymbol.U32;
        if (fromType == TypeSymbol.U32)
            nextBiggerType = TypeSymbol.U64;
        if (fromType == TypeSymbol.U64)
            nextBiggerType = TypeSymbol.U64;

        return nextBiggerType is not null;
    }
}