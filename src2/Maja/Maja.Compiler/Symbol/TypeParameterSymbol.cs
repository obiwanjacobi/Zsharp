namespace Maja.Compiler.Symbol;

public sealed record TypeParameterSymbol : Symbol
{
    public TypeParameterSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.TypeParameter;
}
