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

        public static AstSymbolEntry Add(this AstSymbolTable symbolTable, AstVariable variable)
        {
            var name = variable.Identifier?.Name
                ?? throw new ArgumentException("No identifier name for variable.", nameof(variable));

            var entry = symbolTable.AddSymbol(name, AstSymbolKind.Variable, variable);
            if (!variable.SetSymbol(entry))
            {
                throw new InvalidOperationException(
                    "Add Symbol failed because the Symbol was set on the Variable.");
            }
            return entry;
        }

        public static AstSymbolEntry Add(this AstSymbolTable symbolTable, AstFunction function)
        {
            var name = function.Identifier?.Name
                ?? throw new ArgumentException("No identifier name for function.", nameof(function));

            var entry = symbolTable.AddSymbol(name, AstSymbolKind.Function, function);
            if (!function.SetSymbol(entry))
            {
                throw new InvalidOperationException(
                    "Add Symbol failed because the Symbol was set on the Function.");
            }
            return entry;
        }
    }
}
