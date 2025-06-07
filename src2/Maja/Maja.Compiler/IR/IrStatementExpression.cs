using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrStatementExpression : IrStatement, IrContainer
{
    public IrStatementExpression(StatementExpressionSyntax syntax, IrExpression expression, IrLocality locality)
        : base(syntax, locality)
    {
        Expression = expression;
    }

    public IrExpression Expression { get; }

    public new StatementExpressionSyntax Syntax
        => (StatementExpressionSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Expression.GetDescendantsOfType<T>();
}