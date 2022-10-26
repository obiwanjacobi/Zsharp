namespace Maja.Compiler.Symbol;

public record VariableSymbol : Symbol
{
    public VariableSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Variable;
}
