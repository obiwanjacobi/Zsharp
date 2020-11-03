namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }
    }
}