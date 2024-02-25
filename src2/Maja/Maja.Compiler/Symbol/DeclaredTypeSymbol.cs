using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Maja.Compiler.Symbol;

public record DeclaredTypeSymbol : TypeSymbol
{
    protected DeclaredTypeSymbol(SymbolName name)
        : base(name)
    { }

    public DeclaredTypeSymbol(SymbolName name,
        IEnumerable<TypeParameterSymbol> typeParameters,
        IEnumerable<EnumSymbol> enums,
        IEnumerable<FieldSymbol> fields,
        IEnumerable<RuleSymbol> rules)
        : base(name)
    {
        TypeParameters = typeParameters.ToImmutableArray();
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
    }

    public ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
    public virtual ImmutableArray<EnumSymbol> Enums { get; }
    public virtual ImmutableArray<FieldSymbol> Fields { get; }
    public ImmutableArray<RuleSymbol> Rules { get; }
}

public static class DeclaredTypeSymbolExtensions
{
    public static bool TryLookup(this ImmutableArray<FieldSymbol> fields, string name, [NotNullWhen(true)] out FieldSymbol? field)
    {
        field = fields.FirstOrDefault(f => f.Name.Value == name);
        return field is not null;
    }
}