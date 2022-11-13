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

    public TypeSymbol? GetPreferredType()
    {
        TypeSymbol? type = null;
        if (Candidates.Contains(TypeSymbol.I64))
            type = TypeSymbol.I64;
        else if (Candidates.Contains(TypeSymbol.I32))
            type = TypeSymbol.I32;
        else if (Candidates.Contains(TypeSymbol.U64))
            type = TypeSymbol.U64;
        else if (Candidates.Contains(TypeSymbol.U32))
            type = TypeSymbol.U32;
        else if (Candidates.Contains(TypeSymbol.I16))
            type = TypeSymbol.I16;
        else if (Candidates.Contains(TypeSymbol.U16))
            type = TypeSymbol.U16;
        else if (Candidates.Contains(TypeSymbol.F64))
            type = TypeSymbol.F64;
        else if (Candidates.Contains(TypeSymbol.F32))
            type = TypeSymbol.F32;
        else if (Candidates.Contains(TypeSymbol.F96))
            type = TypeSymbol.F96;
        else if (Candidates.Contains(TypeSymbol.I8))
            type = TypeSymbol.I8;
        else if (Candidates.Contains(TypeSymbol.U8))
            type = TypeSymbol.U8;

        return type;
    }
}
