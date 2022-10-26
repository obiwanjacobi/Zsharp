namespace Maja.Compiler.Symbol;

public abstract record Symbol
{
    protected Symbol(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract SymbolKind Kind { get; }
}
