namespace Maja.Compiler.Symbol;

public sealed record ParameterSymbol : Symbol
{
    public const string Self = "self";

    public ParameterSymbol(string name, TypeSymbol type)
        : base(name)
    {
        Type = type;
    }

    public override SymbolKind Kind
        => SymbolKind.Parameter;

    public TypeSymbol Type { get; }
}
