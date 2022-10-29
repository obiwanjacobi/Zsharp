namespace Maja.Compiler.Symbol;

public sealed record AssemblySymbol : Symbol
{
    public AssemblySymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Assembly;
}
