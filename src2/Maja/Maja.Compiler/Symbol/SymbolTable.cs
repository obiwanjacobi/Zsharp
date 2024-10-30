using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Maja.Compiler.Symbol;

/// <summary>
/// Store declared symbols for lookup.
/// </summary>
internal sealed class SymbolTable
{
    private readonly Dictionary<string, Symbol> _table = new();

    public IEnumerable<Symbol> Symbols
        => _table.Values ?? Enumerable.Empty<Symbol>();

    public bool TryDeclareSymbol<T>(T symbol)
        where T : Symbol
    {
        if (_table.ContainsKey(symbol.Name.Value))
            return false;

        _table.Add(symbol.Name.Value, symbol);
        return true;
    }

    public bool TryDeclareVariable(VariableSymbol symbol)
        => TryDeclareSymbol(symbol);

    public bool TryDeclareType(TypeSymbol symbol)
        => TryDeclareSymbol(symbol);

    public bool TryDeclareFunction(FunctionSymbol symbol)
        => TryDeclareSymbol(symbol);

    public bool TryLookupSymbol(string name, [NotNullWhen(true)] out Symbol? symbol)
        => _table.TryGetValue(name, out symbol);

    public bool TryLookupSymbol(SymbolName name, [NotNullWhen(true)] out Symbol? symbol)
        => _table.TryGetValue(name.Value, out symbol);

    public bool TryLookupSymbol<T>(string name, [NotNullWhen(true)] out T? symbol)
        where T : Symbol
    {
        if (TryLookupSymbol(name, out var genSym) &&
            genSym is T typedSym)
        {
            symbol = typedSym;
            return true;
        }

        symbol = null;
        return false;
    }

    public bool TryLookupSymbol<T>(SymbolName name, [NotNullWhen(true)] out T? symbol)
        where T : Symbol
        => TryLookupSymbol<T>(name.Value, out symbol);
}
