namespace Maja.Compiler.Symbol;

public record DeclaredVariableSymbol : SymbolWithType
{
    public DeclaredVariableSymbol(SymbolName name, TypeSymbol type)
        : base(name, type)
    { }

    public override SymbolKind Kind
        => SymbolKind.Variable;
}
