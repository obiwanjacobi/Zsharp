namespace Maja.Compiler.Symbol;

public sealed record ParameterSymbol : SymbolWithType
{
    public const string Self = "self";

    public ParameterSymbol(string name, TypeSymbol type)
        : base(new SymbolName(name), type)
    { }

    public override SymbolKind Kind
        => SymbolKind.Parameter;
}
