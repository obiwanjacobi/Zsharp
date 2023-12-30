using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExpressionMemberAccess : IrExpression
{
    public IrExpressionMemberAccess(ExpressionSyntax syntax, TypeSymbol type, IrExpression expression, IEnumerable<FieldSymbol> members)
        : base(syntax, type)
    {
        Expression = expression;
        Members = members.ToImmutableArray();
    }

    public IrExpression Expression { get; }

    public ImmutableArray<FieldSymbol> Members { get; }
}
