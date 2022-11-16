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
}
