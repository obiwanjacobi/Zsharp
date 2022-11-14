namespace Maja.Compiler.Symbol;

public sealed record EnumSymbol : Symbol
{
    public EnumSymbol(SymbolName name, TypeSymbol type, object value)
        : base(name)
    {
        Type = type;
        Value = value;
    }

    public override SymbolKind Kind
        => SymbolKind.Enum;

    public TypeSymbol Type { get; }
    public object Value { get; }
}
