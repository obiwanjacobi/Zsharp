namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }
    }

    public static class AstSymbolTableSiteExtensions
    {
        public static AstSymbolEntry AddSymbol(this IAstSymbolTableSite symbolTableSite, string symbolName, AstSymbolKind kind, AstNode? node = null)
            => symbolTableSite.Symbols.AddSymbol(symbolName, kind, node);

        public static void ThrowIfSymbolTableNotSet(this IAstSymbolTableSite symbolTableSite)
        {
            if (symbolTableSite.Symbols is null)
                throw new InternalErrorException("Symbol Table was not set.");
        }
    }
}