using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTypeMatcher
    {
        public static AstFunctionDefinition? ResolveOverloads(AstFunctionReference function)
        {
            if (function?.Symbol is null)
                throw new InternalErrorException("No Symbol is set on the Function Reference.");

            return function.Symbol!.Overloads
                .Select(functionDef => new RankInfo<AstFunctionDefinition>
                { Object = functionDef, Rank = Rank(function, functionDef) })
                .Where(rank => rank.Rank >= 0)
                .OrderByDescending(rank => rank.Rank)
                .Select(r => r.Object)
                .FirstOrDefault();
        }

        public static int Rank(AstFunctionReference functionRef, AstFunctionDefinition functionDef)
        {
            var sources = functionRef.FunctionType.Parameters.Select(p => p.TypeReference!).ToList();
            var targets = functionDef.FunctionType.Parameters.Select(p => p.TypeReference!).ToList();

            if (functionRef.FunctionType.TypeReference is not null &&
                functionDef.FunctionType.TypeReference is not null)
            {
                sources.Add(functionRef.FunctionType.TypeReference);
                targets.Add(functionDef.FunctionType.TypeReference);
            }

            return Rank(sources, targets);
        }

        public static int Rank(IList<AstTypeReference> sources, IList<AstTypeReference> targets)
        {
            if (sources.Count != targets.Count)
                throw new InternalErrorException("Number of TypeReferences and TypeDefinitions must be the same.");

            int rank = 0;
            for (int i = 0; i < sources.Count; i++)
            {
                rank += Rank(sources[i], targets[i]);
            }

            return rank;
        }

        public static int Rank(AstTypeReference source, AstTypeReference target)
        {
            var sourceName = source.Identifier!.CanonicalName;
            var targetName = target.Identifier!.CanonicalName;

            if (sourceName == targetName)
                return 100;

            if (_typeRanks.TryGetValue($"{sourceName}-{targetName}", out int rank))
                return rank;

            return -100;
        }

        private static readonly Dictionary<string, int> _typeRanks = new()
        {
            // "from-to" = rank
            ["I8-I16"] = 75,
            ["I8-I32"] = 75,
            ["I8-I64"] = 75,
            ["I16-I32"] = 75,
            ["I16-I64"] = 75,
            ["I32-I64"] = 75,

            ["I8-U8"] = 45,
            ["I8-U16"] = 50,
            ["I8-U32"] = 50,
            ["I8-U64"] = 50,
            ["I16-U16"] = 45,
            ["I16-U32"] = 50,
            ["I16-U64"] = 50,
            ["I32-U32"] = 45,
            ["I32-U64"] = 50,
            ["I64-U64"] = 45,

            ["U8-U16"] = 75,
            ["U8-U32"] = 75,
            ["U8-U64"] = 75,
            ["U16-U32"] = 75,
            ["U16-U64"] = 75,
            ["U32-U64"] = 75,

            ["U8-I8"] = 45,
            ["U8-I16"] = 50,
            ["U8-I32"] = 50,
            ["U8-I64"] = 50,
            ["U16-I16"] = 45,
            ["U16-I32"] = 50,
            ["U16-I64"] = 50,
            ["U32-I32"] = 45,
            ["U32-I64"] = 50,
            ["U64-I64"] = 45,

            ["F32-F64"] = 75,
            ["F32-F96"] = 75,
            ["F64-F96"] = 75,
        };

        private struct RankInfo<T>
        {
            public T Object;
            public int Rank;
        }
    }
}
