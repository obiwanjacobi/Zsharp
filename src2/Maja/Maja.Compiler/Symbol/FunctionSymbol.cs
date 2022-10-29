using System.Collections.Generic;

namespace Maja.Compiler.Symbol;

public sealed record FunctionSymbol : Symbol
{
    public FunctionSymbol(string name,
        IEnumerable<ParameterSymbol> parameters, TypeSymbol? returnType)
        : base(name)
    {
        Parameters = parameters;
        ReturnType = returnType;
    }

    public override SymbolKind Kind
        => SymbolKind.Function;

    public IEnumerable<ParameterSymbol> Parameters { get; }
    public TypeSymbol? ReturnType { get; }
}

