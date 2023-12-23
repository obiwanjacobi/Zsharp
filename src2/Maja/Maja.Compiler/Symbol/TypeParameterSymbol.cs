namespace Maja.Compiler.Symbol;

public sealed record TypeParameterSymbol : TypeSymbol
{
    public TypeParameterSymbol(string name)
        : base(new SymbolName(name))
    { }

    public override SymbolKind Kind
        => SymbolKind.TypeParameter;
}
