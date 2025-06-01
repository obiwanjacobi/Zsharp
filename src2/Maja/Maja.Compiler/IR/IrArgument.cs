using System.Collections.Generic;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrArgument : IrNode, IrContainer
{
    public IrArgument(ArgumentSyntax syntax,
        IrExpression expression, ParameterSymbol? symbol)
        : base(syntax)
    {
        Expression = expression;
        Symbol = symbol;
    }

    public IrExpression Expression { get; }
    public ParameterSymbol? Symbol { get; }

    public new ArgumentSyntax Syntax
        => (ArgumentSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Expression.GetDescendentsOfType<T>();
}