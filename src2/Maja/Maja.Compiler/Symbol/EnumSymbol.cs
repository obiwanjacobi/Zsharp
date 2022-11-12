namespace Maja.Compiler.Symbol;

public sealed record EnumSymbol : Symbol
{
    public EnumSymbol(SymbolName name, TypeSymbol type)
        : base(name)
    {
        Type = type;
    }

    public override SymbolKind Kind
        => SymbolKind.Enum;

    public TypeSymbol Type { get; }
}
