namespace Maja.Compiler.Symbol;

public record TypeSymbol : Symbol
{
    internal static readonly TypeSymbol Unknown = new("<unknown>", 0);

    // built-in types
    public static readonly TypeSymbol Void = new("Void", 0);
    public static readonly TypeSymbol Bool = new("Bool", 1);
    public static readonly TypeSymbol U8 = new("U8", 1);
    public static readonly TypeSymbol I8 = new("I8", 1);
    public static readonly TypeSymbol U16 = new("U16", 2);
    public static readonly TypeSymbol I16 = new("I16", 2);
    public static readonly TypeSymbol U32 = new("U32", 4);
    public static readonly TypeSymbol I32 = new("I32", 4);
    public static readonly TypeSymbol U64 = new("U64", 8);
    public static readonly TypeSymbol I64 = new("I64", 8);
    public static readonly TypeSymbol F16 = new("F16", 2);
    public static readonly TypeSymbol F32 = new("F32", 4);
    public static readonly TypeSymbol F64 = new("F64", 8);
    public static readonly TypeSymbol F96 = new("F96", 12);
    public static readonly TypeSymbol C16 = new("C16", 2);
    public static readonly TypeSymbol Str = new("Str", 0);

    private TypeSymbol(string name, int sizeInBytes)
        : base(new SymbolName(name))
    {
        SizeInBytes = sizeInBytes;
    }

    public TypeSymbol(string ns, string name)
        : base(new SymbolName(ns, name))
    { }

    public TypeSymbol(SymbolName name,
        int sizeInBytes)
        : base(name)
    {
        SizeInBytes = sizeInBytes;
    }

    public override SymbolKind Kind
        => SymbolKind.Type;
    public virtual bool IsExternal => false;
    public virtual int SizeInBytes { get; }
}
