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
                throw new InternalErrorException(
                    "SymbolEntry is already set or null.");
        }

        public static void ThrowIfSymbolEntryNotSet(this IAstSymbolEntrySite symbolEntrySite)
        {
            if (symbolEntrySite.Symbol == null)
                throw new InternalErrorException("Symbol Entry not set.");
        }
    }
}