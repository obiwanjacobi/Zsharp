namespace Maja.Compiler.Symbol;

public sealed record FieldSymbol : Symbol
{
    public FieldSymbol(SymbolName name, TypeSymbol type)
        : base(name)
    {
        Type = type;
    }

    public override SymbolKind Kind
        => SymbolKind.Field;

    public TypeSymbol Type { get; }
}
