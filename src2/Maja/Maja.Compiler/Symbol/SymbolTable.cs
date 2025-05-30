using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Maja.Compiler.Symbol;

/// <summary>
/// Store declared symbols for lookup.
/// </summary>
internal sealed class SymbolTable
{
    private readonly Dictionary<string, Symbol> _table = new();

    public IReadOnlyCollection<Symbol> Symbols
        => _table.Values;

    public bool TryDeclareSymbol<T>(T symbol)
        where T : Symbol
    {
        var name = symbol.Name.FullName;

        if (_table.ContainsKey(name))
            return false;

        _table.Add(name, symbol);
        return true;
    }

    public bool TryLookupSymbol<T>(string name, [NotNullWhen(true)] out T? symbol)
        where T : Symbol
    {
        if (_table.TryGetValue(name, out var genSymbol) &&
            genSymbol is T typedSym)
        {
            symbol = typedSym;
            return true;
        }

        symbol = null;
        return false;
    }
}
