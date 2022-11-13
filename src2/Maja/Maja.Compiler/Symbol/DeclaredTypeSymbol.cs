using System.Collections.Generic;
using System.Collections.Immutable;

namespace Maja.Compiler.Symbol;

public record DeclaredTypeSymbol : TypeSymbol
{
    public DeclaredTypeSymbol(SymbolName name,
        IEnumerable<EnumSymbol> enums,
        IEnumerable<FieldSymbol> fields,
        IEnumerable<RuleSymbol> rules,
        int sizeInBytes)
        : base(name, sizeInBytes)
    {
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
    }

    internal ImmutableArray<EnumSymbol> Enums { get; }
    internal ImmutableArray<FieldSymbol> Fields { get; }
    internal ImmutableArray<RuleSymbol> Rules { get; }
}
