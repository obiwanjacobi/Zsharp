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
            .Where(fn => partialName.MatchesWith(fn.Name) >= 0
                && fn.Parameters.Count() == argTypeList.Count
                && MatchArgumentTypes(argTypeList, fn.Parameters.Select(p => p.Type)));

        return candidates.ToArray();
    }

    public static bool MatchArgumentTypes(List<TypeSymbol> argTypeList, IEnumerable<TypeSymbol> paramTypes)
    {
        if (argTypeList.SequenceEqual(paramTypes))
            return true;

        var i = 0;
        foreach (var paramType in paramTypes)
        {
            var argType = argTypeList[i];

            if (argType is TypeInferredSymbol inferredType)
            {
                if (!inferredType.Candidates.Contains(paramType))
                    return false;
            }
            else if (argType != paramType)
                return false;

            i++;
        }

        return true;
    }
}
