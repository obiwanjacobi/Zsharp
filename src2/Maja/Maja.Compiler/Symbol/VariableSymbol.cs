namespace Maja.Compiler.Symbol;

public record VariableSymbol : Symbol
{
    public VariableSymbol(SymbolName name, TypeSymbol? type)
        : base(name)
    {
        Type = type;
    }

    public override SymbolKind Kind
        => SymbolKind.Variable;

    public TypeSymbol? Type { get; }
}
