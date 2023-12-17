using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Maja.Compiler.Symbol;

public record TypeInferredSymbol : TypeSymbol
{
    public TypeInferredSymbol(IEnumerable<TypeSymbol> candidates)
        : base(TypeSymbol.Unknown)
    {
        Candidates = candidates.ToImmutableList().WithValueSemantics();
    }

    public IImmutableList<TypeSymbol> Candidates { get; }
}
