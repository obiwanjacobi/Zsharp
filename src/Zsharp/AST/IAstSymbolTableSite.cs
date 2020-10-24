namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }
    }
}