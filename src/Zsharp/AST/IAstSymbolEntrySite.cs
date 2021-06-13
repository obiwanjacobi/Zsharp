namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbol? Symbol { get; }
        bool TrySetSymbol(AstSymbol? symbolEntry);
    }

    public static class AstSymbolEntrySiteExtensions
    {
        public static void SetSymbol(this IAstSymbolEntrySite symbolEntrySite, AstSymbol symbolEntry)
        {
            if (!symbolEntrySite.TrySetSymbol(symbolEntry))
                throw new InternalErrorException(
                    "SymbolEntry is already set or null.");
        }

        public static void ThrowIfSymbolEntryNotSet(this IAstSymbolEntrySite symbolEntrySite)
        {
            if (symbolEntrySite.Symbol is null)
                throw new InternalErrorException("Symbol not set.");
        }
    }
}