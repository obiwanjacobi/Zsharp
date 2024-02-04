using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maja.Compiler.Symbol;

public sealed record FunctionSymbol : Symbol
{
    public FunctionSymbol(SymbolName name,
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<ParameterSymbol> parameters, TypeSymbol? returnType)
        : base(name)
    {
        TypeParameters = typeParameters;
        Parameters = parameters;
        ReturnType = returnType ?? TypeSymbol.Void;

        Type = new TypeFunctionSymbol(
            typeParameters, parameters.Select(p => p.Type).ToList(), returnType);
    }

    public override SymbolKind Kind
        => SymbolKind.Function;

    public IEnumerable<TypeParameterSymbol> TypeParameters { get; }
    public IEnumerable<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }
    public TypeFunctionSymbol Type { get; }
}

public sealed record TypeFunctionSymbol : TypeSymbol
{
    public TypeFunctionSymbol(
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<TypeSymbol> parameterTypes, TypeSymbol? returnType)
        : base(CreateFunctionTypeName(typeParameters, parameterTypes, returnType))
    {
        TypeParameters = typeParameters;
        ParameterTypes = parameterTypes;
        ReturnType = returnType ?? TypeSymbol.Void;
    }

    public override SymbolKind Kind
        => SymbolKind.TypeFunction;

    public IEnumerable<TypeParameterSymbol> TypeParameters { get; }
    public IEnumerable<TypeSymbol> ParameterTypes { get; }
    public TypeSymbol ReturnType { get; }

    private static SymbolName CreateFunctionTypeName(IEnumerable<TypeParameterSymbol> typeParameters,
        IEnumerable<TypeSymbol> parameterTypes, TypeSymbol? returnType)
    {
        var name = new StringBuilder();

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

        var generics = typeParameters.Count();
        if (generics > 0)
        {
            name.Append('`')
                .Append(generics);
        }

        return new SymbolName(name.ToString());
    }
}
