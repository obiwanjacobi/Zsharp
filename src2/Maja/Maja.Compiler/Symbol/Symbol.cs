namespace Maja.Compiler.Symbol;

public abstract record Symbol
{
    protected Symbol()
    {
        Name = SymbolName.Empty;
    }

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
