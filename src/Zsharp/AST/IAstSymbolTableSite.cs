using System;

namespace Zsharp.AST
{
    public interface IAstSymbolTableSite
    {
        AstSymbolTable Symbols { get; }
        AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(symbolName, kind, node);
        }

        AstSymbolEntry AddSymbol(AstIdentifier identifier, AstSymbolKind kind, AstNode node)
        {
            return Symbols.AddSymbol(identifier.Name, kind, node);
        }

        AstSymbolEntry AddSymbol(IAstIdentifierSite identifierSite, AstSymbolKind kind, AstNode node)
        {
            if (identifierSite?.Identifier == null)
                throw new ArgumentNullException(nameof(identifierSite));
            return Symbols.AddSymbol(identifierSite.Identifier.Name, kind, node);
        }
    }
}