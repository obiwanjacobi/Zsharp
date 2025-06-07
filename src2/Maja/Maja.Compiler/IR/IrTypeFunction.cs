using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrTypeFunction : IrNode, IrContainer
{
    public IrTypeFunction(SyntaxNode syntax, TypeFunctionSymbol symbol,
        IEnumerable<IrTypeParameter> typeParameters, IEnumerable<IrType> parameterTypes, IrType returnType)
        : base(syntax)
    {
        Symbol = symbol;
        TypeParameters = typeParameters.ToImmutableArray();
        ParameterTypes = parameterTypes.ToImmutableArray();
        ReturnType = returnType;
    }

    public TypeFunctionSymbol Symbol { get; }
    public ImmutableArray<IrTypeParameter> TypeParameters { get; }
    public ImmutableArray<IrType> ParameterTypes { get; }
    public IrType ReturnType { get; }

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => TypeParameters.GetDescendantsOfType<T>()
        .Concat(ParameterTypes.GetDescendantsOfType<T>())
        .Concat(ReturnType.GetDescendantsOfType<T>());
}
