namespace Maja.Compiler.Symbol;

public abstract record TypeParameterSymbol : TypeSymbol
{
    protected TypeParameterSymbol(SymbolName name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.TypeParameter;
}

public sealed record TypeParameterGenericSymbol : TypeParameterSymbol
{
    public TypeParameterGenericSymbol(string name)
        : base(new SymbolName(name))
    { }
}

public sealed record TypeParameterTemplateSymbol : TypeParameterSymbol
{
    public TypeParameterTemplateSymbol(string name)
        : base(new SymbolName(name))
    { }
}
