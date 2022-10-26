namespace Maja.Compiler.Symbol;

public sealed record ParameterSymbol : Symbol
{
    public ParameterSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Parameter;
}

