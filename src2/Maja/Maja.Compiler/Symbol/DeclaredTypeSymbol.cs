using System.Collections.Generic;
using System.Collections.Immutable;

namespace Maja.Compiler.Symbol;

public record DeclaredTypeSymbol : TypeSymbol
{
    protected DeclaredTypeSymbol(SymbolName name)
        : base(name)
    { }

    public DeclaredTypeSymbol(SymbolName name,
    IEnumerable<EnumSymbol> enums,
    IEnumerable<FieldSymbol> fields,
    IEnumerable<RuleSymbol> rules)
    : base(name)
    {
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
    }

    public virtual ImmutableArray<EnumSymbol> Enums { get; }
    public virtual ImmutableArray<FieldSymbol> Fields { get; }
    public virtual ImmutableArray<RuleSymbol> Rules { get; }
}
