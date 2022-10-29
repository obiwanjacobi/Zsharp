namespace Maja.Compiler.Symbol;

public sealed record AliasSymbol : Symbol
{
    public AliasSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Alias;
}
