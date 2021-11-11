namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable SymbolTable { get; }
    }

    public static class AstSymbolTableSiteExtensions
    {
        public static AstSymbol AddSymbol(this IAstSymbolTableSite symbolTableSite, string symbolName, AstSymbolKind kind, AstNode? node = null)
            => symbolTableSite.SymbolTable.AddSymbol(symbolName, kind, node);

        public static void ThrowIfSymbolTableNotSet(this IAstSymbolTableSite symbolTableSite)
        {
            if (symbolTableSite.SymbolTable is null)
                throw new InternalErrorException("Symbol Table was not set.");
        }
    }
}