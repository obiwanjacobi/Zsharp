namespace Maja.Compiler.Symbol;

public sealed record ModuleSymbol : Symbol
{
    public ModuleSymbol(SymbolName name)
        : base(name.FullName)
    {
        SymbolName = name;
    }

    public override SymbolKind Kind
        => SymbolKind.Module;

    public SymbolName SymbolName { get; }
}
