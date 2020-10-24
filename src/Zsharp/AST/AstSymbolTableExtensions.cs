﻿using System;

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

            if (node is IAstSymbolEntrySite symbolSite &&
                !symbolSite.SetSymbol(entry))
            {
                throw new InvalidOperationException(
                    "Add Symbol failed because the Symbol was set on the Function.");
            }
            return entry;
        }
    }
}
