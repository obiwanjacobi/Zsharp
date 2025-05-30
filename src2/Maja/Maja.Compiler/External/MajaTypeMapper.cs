using System;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal static class MajaTypeMapper
{
    public static TypeSymbol? MapToMajaType(string ns, string name)
    {
        TypeSymbol? majaType = null;

        if (ns == "System")
        {
            majaType = name switch
            {
                "Boolean" => TypeSymbol.Bool,
                "Byte" => TypeSymbol.U8,
                "SByte" => TypeSymbol.I8,
                "Uint16" => TypeSymbol.U16,
                "Int16" => TypeSymbol.I16,
                "UInt32" => TypeSymbol.U32,
                "Int32" => TypeSymbol.I32,
                "Uint64" => TypeSymbol.U64,
                "Int64" => TypeSymbol.I64,
                "Char" => TypeSymbol.C16,
                "String" => TypeSymbol.Str,
                "Void" => TypeSymbol.Void,
                "Half" => TypeSymbol.F16,
                "Single" => TypeSymbol.F32,
                "Double" => TypeSymbol.F64,
                "Decimal" => TypeSymbol.F96,
                _ => null
            };
        }

        return majaType;
    }

    public static string MapToDotNetType(TypeSymbol majaType)
    {
        if (majaType == TypeSymbol.Bool)
            return typeof(bool).FullName!;
        else if (majaType == TypeSymbol.U8)
            return typeof(byte).FullName!;
        else if (majaType == TypeSymbol.I8)
            return typeof(sbyte).FullName!;
        else if (majaType == TypeSymbol.U16)
            return typeof(ushort).FullName!;
        else if (majaType == TypeSymbol.I16)
            return typeof(short).FullName!;
        else if (majaType == TypeSymbol.U32)
            return typeof(uint).FullName!;
        else if (majaType == TypeSymbol.I32)
            return typeof(int).FullName!;
        else if (majaType == TypeSymbol.U64)
            return typeof(ulong).FullName!;
        else if (majaType == TypeSymbol.I64)
            return typeof(long).FullName!;
        else if (majaType == TypeSymbol.C16)
            return typeof(char).FullName!;
        else if (majaType == TypeSymbol.Str)
            return typeof(string).FullName!;
        else if (majaType == TypeSymbol.F16)
            return typeof(Half).FullName!;
        else if (majaType == TypeSymbol.F32)
            return typeof(float).FullName!;
        else if (majaType == TypeSymbol.F64)
            return typeof(double).FullName!;
        else if (majaType == TypeSymbol.F96)
            return typeof(decimal).FullName!;
        else if (majaType == TypeSymbol.Void)
            return "void";

        return majaType.Name.FullName
            .Replace('#', '_');     // replace template instantiations markers
    }
}
