namespace Maja.Compiler.Symbol;

public sealed record ModuleSymbol : Symbol
{
    public ModuleSymbol(string name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Module;
}

