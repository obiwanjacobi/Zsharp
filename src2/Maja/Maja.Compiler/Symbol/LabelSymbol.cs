namespace Maja.Compiler.Symbol;

public sealed record LabelSymbol : Symbol
{
    public LabelSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Label;
}
