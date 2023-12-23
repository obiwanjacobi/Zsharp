using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Maja.Compiler.Symbol;

internal sealed class SymbolMap<K, V>
    where K : Symbol
    where V : Symbol
{
    private readonly Dictionary<K, V> _map = new();

    public void AddRange(IList<K> keySymbols, IList<V> valueSymbols, V? defaultSymbol = null)
    {
        for (var i = 0; i < keySymbols.Count; i++)
        {
            var keySymbol = keySymbols[i];

            if (i < valueSymbols.Count)
            {
                _map[keySymbol] = valueSymbols[i];
            }
            else if (defaultSymbol is not null)
                _map[keySymbol] = defaultSymbol;
        }
    }

    public bool TryMapSymbol(K? keySymbol, [NotNullWhen(true)] out V? valueSymbol)
    {
        if (keySymbol is null)
        {
            valueSymbol = null;
            return false;
        }

        return _map.TryGetValue(keySymbol, out valueSymbol);
    }
}
