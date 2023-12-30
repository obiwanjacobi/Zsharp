namespace Maja.Compiler.Symbol;

public record VariableSymbol : SymbolWithType
{
    public VariableSymbol(SymbolName name, TypeSymbol type)
        : base(name, type)
    { }

    public override SymbolKind Kind
        => SymbolKind.Variable;
}
