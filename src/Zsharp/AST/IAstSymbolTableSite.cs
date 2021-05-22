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
    }
}