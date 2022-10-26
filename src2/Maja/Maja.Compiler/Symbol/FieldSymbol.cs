namespace Maja.Compiler.Symbol;

public sealed record FieldSymbol : Symbol
{
    public FieldSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Field;
}

