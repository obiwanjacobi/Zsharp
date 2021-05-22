namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool TrySetSymbol(AstSymbolEntry? symbolEntry);
        bool TryResolve();
    }

    public static class AstSymbolEntrySiteExtensions
    {
        public static void SetSymbol(this IAstSymbolEntrySite symbolEntrySite, AstSymbolEntry symbolEntry)
        {
            if (!symbolEntrySite.TrySetSymbol(symbolEntry))
                throw new ZsharpException(
                    "SymbolEntry is already set or null.");
        }
    }
}