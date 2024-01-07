namespace Maja.Compiler.Symbol;

public record TypeSymbol : Symbol
{
    internal static readonly TypeSymbol Unknown = new("<unknown>");

    // built-in types
    public static readonly TypeSymbol Void = new("Void");
    public static readonly TypeSymbol Bool = new("Bool");
    public static readonly TypeSymbol U8 = new("U8");
    public static readonly TypeSymbol I8 = new("I8");
    public static readonly TypeSymbol U16 = new("U16");
    public static readonly TypeSymbol I16 = new("I16");
    public static readonly TypeSymbol U32 = new("U32");
    public static readonly TypeSymbol I32 = new("I32");
    public static readonly TypeSymbol U64 = new("U64");
    public static readonly TypeSymbol I64 = new("I64");
    public static readonly TypeSymbol F16 = new("F16");
    public static readonly TypeSymbol F32 = new("F32");
    public static readonly TypeSymbol F64 = new("F64");
    public static readonly TypeSymbol F96 = new("F96");
    public static readonly TypeSymbol C16 = new("C16");
    public static readonly TypeSymbol Str = new("Str");

    private TypeSymbol(string name)
        : base(new SymbolName(name))
    {
        IsWellknown = true;
    }

    public TypeSymbol(string ns, string name)
        : base(new SymbolName(ns, name))
    { }

    public TypeSymbol(SymbolName name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Type;
    public virtual bool IsExternal => false;
    public bool IsWellknown { get; }

    public virtual bool MatchesWith(TypeSymbol other)
    {
        if (other == this) return true;

        if (other == TypeSymbol.I16)
            return this == TypeSymbol.I8;
        if (other == TypeSymbol.I32)
            return this == TypeSymbol.I8 || this == TypeSymbol.I16;
        if (other == TypeSymbol.I64)
            return this == TypeSymbol.I8 || this == TypeSymbol.I16 || this == TypeSymbol.I32;

        if (other == TypeSymbol.U16)
            return this == TypeSymbol.U8;
        if (other == TypeSymbol.U32)
            return this == TypeSymbol.U8 || this == TypeSymbol.U16;
        if (other == TypeSymbol.U64)
            return this == TypeSymbol.U8 || this == TypeSymbol.U16 || this == TypeSymbol.U32;

        if (other == TypeSymbol.F32)
            return this == TypeSymbol.F16;
        if (other == TypeSymbol.F64)
            return this == TypeSymbol.F16 || this == TypeSymbol.F32;
        if (other == TypeSymbol.F96)
            return this == TypeSymbol.F16 || this == TypeSymbol.F32|| this == TypeSymbol.F64;

        return false;
    }

    public static bool IsVoid(TypeSymbol type)
        => type == TypeSymbol.Void;

    public static bool IsBoolean(TypeSymbol type)
        => type == TypeSymbol.Bool;

    public static bool IsI8(TypeSymbol type)
        => type == TypeSymbol.I8;
    public static bool IsU8(TypeSymbol type)
        => type == TypeSymbol.U8;
    public static bool IsI16(TypeSymbol type)
        => type == TypeSymbol.I16;
    public static bool IsU16(TypeSymbol type)
        => type == TypeSymbol.U16;
    public static bool IsI32(TypeSymbol type)
        => type == TypeSymbol.I32;
    public static bool IsU32(TypeSymbol type)
        => type == TypeSymbol.U32;
    public static bool IsI64(TypeSymbol type)
        => type == TypeSymbol.I64;
    public static bool IsU64(TypeSymbol type)
        => type == TypeSymbol.U64;

    public static bool IsInteger(TypeSymbol type)
    {
        return type == TypeSymbol.I8
            || type == TypeSymbol.U8
            || type == TypeSymbol.I16
            || type == TypeSymbol.U16
            || type == TypeSymbol.I32
            || type == TypeSymbol.U32
            || type == TypeSymbol.I64
            || type == TypeSymbol.U64
            ;
    }

    public static bool IsF16(TypeSymbol type)
        => type == TypeSymbol.F16;
    public static bool IsF32(TypeSymbol type)
        => type == TypeSymbol.F32;
    public static bool IsF64(TypeSymbol type)
        => type == TypeSymbol.F64;
    public static bool IsF96(TypeSymbol type)
        => type == TypeSymbol.F96;

    public static bool IsFloat(TypeSymbol type)
    {
        return type == TypeSymbol.F16
            || type == TypeSymbol.F32
            || type == TypeSymbol.F64
            || type == TypeSymbol.F96
            ;
    }

    public static bool IsC16(TypeSymbol type)
        => type == TypeSymbol.C16;
    public static bool IsStr(TypeSymbol type)
        => type == TypeSymbol.Str;
}
