using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Maja.Compiler.Symbol;

public sealed record FunctionSymbol : Symbol
{
    public FunctionSymbol(SymbolName name,
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<ParameterSymbol> parameters, TypeSymbol? returnType)
        : base(name)
    {
        TypeParameters = typeParameters.ToImmutableArray();
        Parameters = parameters.ToImmutableArray();
        ReturnType = returnType ?? TypeSymbol.Void;

        Type = new TypeFunctionSymbol(
            typeParameters, parameters.Select(p => p.Type).ToList(), returnType);
    }

    public override SymbolKind Kind
        => SymbolKind.Function;

    public bool IsTemplate
        => TypeParameters.OfType<TypeParameterTemplateSymbol>().Any();

    public ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
    public ImmutableArray<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }
    public TypeFunctionSymbol Type { get; }
}

public sealed record TypeFunctionSymbol : TypeSymbol
{
    public TypeFunctionSymbol(
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<TypeSymbol> parameterTypes, TypeSymbol? returnType)
        : base(CreateFunctionTypeName(typeParameters, parameterTypes, returnType))
    {
        TypeParameters = typeParameters.ToImmutableArray();
        ParameterTypes = parameterTypes.ToImmutableArray();
        ReturnType = returnType ?? TypeSymbol.Void;
    }

    public override SymbolKind Kind
        => SymbolKind.TypeFunction;

    public bool IsTemplate
        => TypeParameters.OfType<TypeParameterTemplateSymbol>().Any();

    public ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
    public ImmutableArray<TypeSymbol> ParameterTypes { get; }
    public TypeSymbol ReturnType { get; }

    private static SymbolName CreateFunctionTypeName(IEnumerable<TypeParameterSymbol> typeParameters,
        IEnumerable<TypeSymbol> parameterTypes, TypeSymbol? returnType)
    {
        var name = new StringBuilder();

        var templParams = typeParameters.OfType<TypeParameterTemplateSymbol>();
        if (templParams.Any())
        {
            // TODO: are template parameters part of the function-type name?
        }

        name.Append('(');
        foreach (var paramType in parameterTypes)
        {
            name.Append(paramType.Name.Value);
        }
        name.Append(')');

        if (returnType is not null)
        {
            name.Append(':')
                .Append(returnType.Name.Value);
        }

        var generics = typeParameters.OfType<TypeParameterGenericSymbol>().Count();
        if (generics > 0)
        {
            name.Append('`')
                .Append(generics);
        }

        return new SymbolName(name.ToString(), isType: true);
    }
}
