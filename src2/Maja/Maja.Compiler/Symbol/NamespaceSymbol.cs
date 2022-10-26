namespace Maja.Compiler.Symbol;

public sealed record NamespaceSymbol : Symbol
{
    public NamespaceSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Namespace;
}

