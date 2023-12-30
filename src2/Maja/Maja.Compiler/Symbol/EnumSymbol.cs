namespace Maja.Compiler.Symbol;

public sealed record EnumSymbol : SymbolWithType
{
    public EnumSymbol(SymbolName name, TypeSymbol type, object value)
        : base(name, type)
    {
        Value = value;
    }

    public override SymbolKind Kind
        => SymbolKind.Enum;

    public object Value { get; }
}
