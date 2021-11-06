namespace Zsharp.AST
{
    public interface IAstSymbolSite
    {
        bool HasSymbol { get; }
        AstSymbol Symbol { get; }
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
            if (!symbolEntrySite.HasSymbol)
                throw new InternalErrorException("Symbol not set.");
        }
    }
}