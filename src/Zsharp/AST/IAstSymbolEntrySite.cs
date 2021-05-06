namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool TrySetSymbol(AstSymbolEntry? symbolEntry);
        void SetSymbol(AstSymbolEntry symbolEntry);
        bool TryResolve();
    }
}