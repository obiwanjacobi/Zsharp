using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal static class MajaTypeMapper
{
    public static TypeSymbol MapToMajaType(string ns, string name)
    {
        TypeSymbol majaType;

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
                "Float" => TypeSymbol.F32,
                "Double" => TypeSymbol.F64,
                "Decimal" => TypeSymbol.F96,
                _ => new TypeSymbol(ns, name)
            };
        }
        else
            majaType = new TypeSymbol(ns, name);

        return majaType;
    }
}
