namespace Maja.Compiler.Symbol;

public sealed record ModuleSymbol : Symbol
{
    public ModuleSymbol(SymbolName name)
        : base(name)
    { }

    public override SymbolKind Kind
        => SymbolKind.Module;
}
