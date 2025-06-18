namespace Maja.Compiler.Symbol;

public record DeclaredVariableSymbol : SymbolWithType
{
    public DeclaredVariableSymbol(SymbolName name, TypeSymbol type)
        : base(name, type)
    { }

    public override SymbolKind Kind
        => SymbolKind.Variable;
}

public sealed record UnresolvedVariableSymbol : Symbol
{
    public UnresolvedVariableSymbol(SymbolName name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Variable;
}