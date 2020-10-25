using System;

namespace Zsharp.AST
{
    public static class AstSymbolTableExtensions
    {
        public static T? FindDefinition<T>(this AstSymbolTable symbolTable, string symbolName, AstSymbolKind symbolKind)
            where T : class
        {
            var entry = symbolTable.FindEntry(symbolName, symbolKind);
            if (entry != null)
            {
                return entry.DefinitionAs<T>();
            }
            return null;
        }

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, AstType type)
        {
            return symbolTable.FindEntry(type, AstSymbolKind.Type);
        }

        public static AstSymbolEntry? Find(this AstSymbolTable symbolTable, AstVariable variable)
        {
            return symbolTable.FindEntry(variable, AstSymbolKind.Variable);
        }

        public static AstSymbolEntry Add(this AstSymbolTable symbolTable, AstVariable variable)
        {
            return AddSymbol(symbolTable, variable, AstSymbolKind.Variable, variable);
        }

        public static AstSymbolEntry Add(this AstSymbolTable symbolTable, AstFunction function)
        {
            return AddSymbol(symbolTable, function, AstSymbolKind.Function, function);
        }

        private static AstSymbolEntry AddSymbol(AstSymbolTable symbolTable,
            IAstIdentifierSite identifierSite, AstSymbolKind symbolKind, AstNode node)
        {
            var name = identifierSite.Identifier?.Name
                ?? throw new ArgumentException("No identifier name.", nameof(identifierSite));

            var entry = symbolTable.AddSymbol(name, symbolKind, node);

            if (node is IAstSymbolEntrySite symbolSite)
            {
                symbolSite.SetSymbol(entry);
            }
            return entry;
        }
    }
}
