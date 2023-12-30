namespace Maja.Compiler.Symbol;

public abstract record Symbol
{
    protected Symbol(string name)
    {
        Name = new SymbolName(name);
    }

    protected Symbol(SymbolName name)
    {
        Name = name;
    }

    public SymbolName Name { get; }

    public abstract SymbolKind Kind { get; }
}

public abstract record SymbolWithType : Symbol
{
    protected SymbolWithType(SymbolName name, TypeSymbol type)
        : base(name)
    {
        Type = type;
    }

    public TypeSymbol Type { get; }
}
