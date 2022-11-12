using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.IR;

internal static class FunctionOverloadPicker
{
    public static bool TryPickFunctionSymbol(
        IEnumerable<FunctionSymbol> functions,
        SymbolName partialName,
        IEnumerable<TypeSymbol> argumentTypes,
        [NotNullWhen(true)] out FunctionSymbol? function)
    {
        var candidates = SelectCandidates(functions, partialName, argumentTypes);

        if (candidates.Any())
        {
            Debug.Assert(candidates.Count() == 1);

            function = candidates.Single();
            return true;
        }

        function = null;
        return false;
    }

    public static IEnumerable<FunctionSymbol> SelectCandidates(
        IEnumerable<FunctionSymbol> functions,
        SymbolName partialName,
        IEnumerable<TypeSymbol> argumentTypes)
    {
        var argTypeList = argumentTypes.ToList();
        var candidates = functions
            .Where(fn => partialName.Matches(fn.Name) >= 0
                && fn.Parameters.Count() == argTypeList.Count
                && argTypeList.SequenceEqual(fn.Parameters.Select(p => p.Type)));

        return candidates.ToArray();
    }
}
