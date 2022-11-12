using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed class ExternalModule
{
    public ExternalModule(SymbolName symbolName,
        IEnumerable<FunctionSymbol> functions,
        IEnumerable<TypeSymbol> types)
    {
        SymbolName = symbolName;
        Types = types.ToImmutableArray();
        Functions = functions.ToImmutableArray();
    }

    public SymbolName SymbolName { get; }
    public ImmutableArray<TypeSymbol> Types { get; }
    public ImmutableArray<FunctionSymbol> Functions { get; }

    public IEnumerable<FunctionSymbol> LookupFunctions(SymbolName name, IEnumerable<TypeSymbol> argumentTypes)
        => FunctionOverloadPicker.SelectCandidates(Functions, name, argumentTypes);

    public IEnumerable<TypeSymbol> LookupTypes(SymbolName name)
    {
        var results = new List<TypeSymbol>();

        foreach (var tp in Types)
        {
            if (name.MatcheWith(tp.Name) >= 0)
            {
                results.Add(tp);
            }
        }

        return results;
    }
}
