namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool SetSymbol(AstSymbolEntry symbolEntry);
    }
}