namespace Maja.Compiler.Symbol;

public sealed record RuleSymbol : Symbol
{
    public RuleSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Rule;
}
