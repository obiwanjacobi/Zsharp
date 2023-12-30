namespace Maja.Compiler.Symbol;

public sealed record FieldSymbol : SymbolWithType
{
    public FieldSymbol(SymbolName name, TypeSymbol type)
        : base(name, type)
    { }

    public override SymbolKind Kind
        => SymbolKind.Field;
}
