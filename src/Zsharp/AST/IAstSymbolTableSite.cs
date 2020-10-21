namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }

        public AstSymbolEntry AddSymbol(AstIdentifier identifier, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(identifier.Name, kind, node);
        }

        public AstSymbolEntry AddSymbol(IAstIdentifierSite identifierSite, AstSymbolKind kind, AstNode node)
        {
            identifierSite.ThrowIfIdentifierNotSet();
            return Symbols.AddSymbol(identifierSite.Identifier!.Name, kind, node);
        }
    }
}