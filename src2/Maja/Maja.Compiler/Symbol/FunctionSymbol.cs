using System.Collections.Generic;

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
    }

    public override SymbolKind Kind
        => SymbolKind.Function;

    public IEnumerable<TypeParameterSymbol> TypeParameters { get; }
    public IEnumerable<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }
}
