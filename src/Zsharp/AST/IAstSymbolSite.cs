namespace Zsharp.AST
{
    public interface IAstSymbolSite
    {
        AstSymbol? Symbol { get; }
        bool TrySetSymbol(AstSymbol? symbolEntry);
    }

    public static class AstSymbolSiteExtensions
    {
        public static void SetSymbol(this IAstSymbolSite symbolEntrySite, AstSymbol symbolEntry)
        {
            if (!symbolEntrySite.TrySetSymbol(symbolEntry))
                throw new InternalErrorException(
                    "Symbol is already set or null.");
        }

        public static void ThrowIfSymbolNotSet(this IAstSymbolSite symbolEntrySite)
        {
            if (symbolEntrySite.Symbol is null)
                throw new InternalErrorException("Symbol not set.");
        }
    }
}