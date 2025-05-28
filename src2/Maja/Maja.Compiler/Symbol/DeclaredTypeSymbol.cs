using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Maja.Compiler.Symbol;

public record DeclaredTypeSymbol : TypeSymbol
{
    protected DeclaredTypeSymbol(SymbolName name)
        : base(name)
    {
        TemplateName = name;
        TypeParameters = [];
        Enums = [];
        Fields = [];
        Rules = [];
    }

    public DeclaredTypeSymbol(SymbolName name,
        IEnumerable<TypeParameterSymbol> typeParameters,
        IEnumerable<EnumSymbol> enums,
        IEnumerable<FieldSymbol> fields,
        IEnumerable<RuleSymbol> rules,
        TypeSymbol? baseType)
        : base(name)
    {
        TypeParameters = typeParameters.ToImmutableArray();
        Enums = enums.ToImmutableArray();
        Fields = fields.ToImmutableArray();
        Rules = rules.ToImmutableArray();
        BaseType = baseType;
        TemplateName = CreateTypeName(name, typeParameters);
    }

    public override bool IsUnresolved
    {
        get
        {
            return TypeParameters.Any(tp => tp.IsUnresolved) ||
                Fields.Any(f => f.IsUnresolved) ||
                (BaseType?.IsUnresolved ?? false);
        }
    }

    public bool IsGeneric
        => TypeParameters.OfType<TypeParameterGenericSymbol>().Any();
    public bool IsTemplate
        => TypeParameters.OfType<TypeParameterTemplateSymbol>().Any();
    // In case of template declarations this is the full type name
    public SymbolName TemplateName { get; protected set; }

    public ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
    public virtual ImmutableArray<EnumSymbol> Enums { get; }
    public virtual ImmutableArray<FieldSymbol> Fields { get; }
    public ImmutableArray<RuleSymbol> Rules { get; }
    public TypeSymbol? BaseType { get; }

    private static SymbolName CreateTypeName(SymbolName typeName, IEnumerable<TypeSymbol> typeTemplateTypes)
    {
        if (!typeTemplateTypes.Any())
            return typeName;

        var name = new StringBuilder(typeName.FullName)
            .Append('#')
            .Append(typeTemplateTypes.Count());

        return new SymbolName(name.ToString(), isType: true);
    }
}

public record UnresolvedDeclaredTypeSymbol : DeclaredTypeSymbol
{
    public UnresolvedDeclaredTypeSymbol(SymbolName name)
        : base(name, Enumerable.Empty<TypeParameterSymbol>(), Enumerable.Empty<EnumSymbol>(),
            Enumerable.Empty<FieldSymbol>(), Enumerable.Empty<RuleSymbol>(), null)
    { }

    public override bool IsUnresolved => true;
    public override bool TryIsUnresolved([NotNullWhen(true)] out UnresolvedDeclaredTypeSymbol? unresolvedSymbol)
    {
        unresolvedSymbol = this;
        return true;
    }
}

public static class DeclaredTypeSymbolExtensions
{
    public static bool TryLookup(this ImmutableArray<FieldSymbol> fields, string name, [NotNullWhen(true)] out FieldSymbol? field)
    {
        field = fields.FirstOrDefault(f => f.Name.Value == name);
        return field is not null;
    }
}