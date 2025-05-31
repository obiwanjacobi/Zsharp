using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Maja.Compiler.Symbol;

public sealed record TypeTemplateSymbol : DeclaredTypeSymbol
{
    public TypeTemplateSymbol(SymbolName name,
        IEnumerable<TypeSymbol> typeArgumentTypes,
        IEnumerable<EnumSymbol> enums,
        IEnumerable<FieldSymbol> fields,
        IEnumerable<RuleSymbol> rules,
        TypeSymbol? baseType)
        : base(CreateTypeName(name, typeArgumentTypes),
            Enumerable.Empty<TypeParameterSymbol>(), enums, fields, rules, baseType)
    {
        TypeArgumentTypes = typeArgumentTypes.ToImmutableArray();
        //TemplateName = CreateTypeName(name, typeArgumentTypes);
    }

    public ImmutableArray<TypeSymbol> TypeArgumentTypes { get; }

    public override SymbolKind Kind
        => SymbolKind.TypeTemplate;

    private static SymbolName CreateTypeName(SymbolName typeName, IEnumerable<TypeSymbol> typeTemplateTypes)
    {
        if (!typeTemplateTypes.Any())
            return typeName;

        var name = new StringBuilder(typeName.FullOriginalName)
            .Append('#')
            .Append(String.Join(',', typeTemplateTypes.Select(ttt => ttt.Name.Value)));

        return new SymbolName(name.ToString(), isType: true);
    }
}
