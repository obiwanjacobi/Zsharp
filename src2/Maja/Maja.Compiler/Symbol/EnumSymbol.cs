namespace Maja.Compiler.Symbol;

public sealed record EnumSymbol : Symbol
{
    public EnumSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Enum;
}

