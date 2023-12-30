using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExpressionMemberAccess : IrExpression
{
    public IrExpressionMemberAccess(ExpressionSyntax syntax, TypeSymbol type, IrExpressionIdentifier identifier, IEnumerable<FieldSymbol> members)
        : base(syntax, type)
    {
        Identifier = identifier;
        Members = members.ToImmutableArray();
    }

    public IrExpressionIdentifier Identifier { get; }

    public ImmutableArray<FieldSymbol> Members { get; }
}
