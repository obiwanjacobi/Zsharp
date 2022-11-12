using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Maja.Compiler.Symbol;

/// <summary>
/// Symbol Table functionality on an optional external dictionary.
/// </summary>
internal static class SymbolTable
{
    public static bool TryDeclareSymbol<T>(ref Dictionary<string, Symbol>? table, T symbol)
        where T : Symbol
    {
        table ??= new Dictionary<string, Symbol>();

        if (table.ContainsKey(symbol.Name.FullName))
            return false;

        table.Add(symbol.Name.FullName, symbol);
        return true;
    }

    public static bool TryLookupSymbol(Dictionary<string, Symbol>? table, string name,
        [NotNullWhen(true)] out Symbol? symbol)
    {
        if (table is null)
        {
            symbol = null;
            return false;
        }

        return table.TryGetValue(name, out symbol);
    }

    public static bool TryLookupSymbol<T>(Dictionary<string, Symbol>? table, string name,
        [NotNullWhen(true)] out T? symbol)
        where T : Symbol
    {
        if (TryLookupSymbol(table, name, out var genSym) &&
            genSym is T typedSym)
        {
            symbol = typedSym;
            return true;
        }

        symbol = null;
        return false;
    }
}
