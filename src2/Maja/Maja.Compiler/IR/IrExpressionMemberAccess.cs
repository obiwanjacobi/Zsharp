using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExpressionMemberAccess : IrExpression, IrContainer
{
    public IrExpressionMemberAccess(ExpressionSyntax syntax, TypeSymbol type, IrExpression expression, IEnumerable<FieldSymbol> members)
        : base(syntax, type)
    {
        Expression = expression;
        Members = members.ToImmutableArray();
    }

    public IrExpression Expression { get; }

    /// <summary>
    /// Expression as identifier.
    /// </summary>
    public IrExpressionIdentifier? Identifier
        => Expression as IrExpressionIdentifier;

    /// <summary>
    /// Expression as invocation.
    /// </summary>
    public IrExpressionInvocation? Invocation
        => Expression as IrExpressionInvocation;

    public ImmutableArray<FieldSymbol> Members { get; }

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Expression.GetDescendantsOfType<T>();
}
