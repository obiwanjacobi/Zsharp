namespace Maja.Compiler.Symbol;

public sealed record RuleSymbol : Symbol
{
    public RuleSymbol(SymbolName name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Rule;
}
