﻿using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrStatementExpression : IrStatement
{
    public IrStatementExpression(StatementExpressionSyntax syntax, IrExpression expression, IrLocality locality)
        : base(syntax, locality)
    {
        Expression = expression;
    }

    public IrExpression Expression { get; }

    public new StatementExpressionSyntax Syntax
        => (StatementExpressionSyntax)base.Syntax;
}