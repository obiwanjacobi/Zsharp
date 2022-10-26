namespace Maja.Compiler.Symbol;

public sealed record FunctionSymbol : Symbol
{
    public FunctionSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Function;
}

