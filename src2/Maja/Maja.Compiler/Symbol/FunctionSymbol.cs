using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Symbol;

public sealed record FunctionSymbol : Symbol
{
    public FunctionSymbol(SymbolName name,
        IEnumerable<TypeParameterSymbol> typeParameters, IEnumerable<ParameterSymbol> parameters, TypeSymbol? returnType)
        : base(name)
    {
        _typeParameters = new List<TypeParameterSymbol>(typeParameters);
        Parameters = parameters;
        ReturnType = returnType ?? TypeSymbol.Void;
    }

    public override SymbolKind Kind
        => SymbolKind.Function;

    private readonly List<TypeParameterSymbol> _typeParameters;
    public IEnumerable<TypeParameterSymbol> TypeParameters => _typeParameters;
    public IEnumerable<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }
}
