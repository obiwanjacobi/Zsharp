namespace Maja.Compiler.Symbol;

public sealed record DiscardSymbol : Symbol
{
    public DiscardSymbol()
        : base("_")
    { }

    public override SymbolKind Kind
        => SymbolKind.Discard;
}
