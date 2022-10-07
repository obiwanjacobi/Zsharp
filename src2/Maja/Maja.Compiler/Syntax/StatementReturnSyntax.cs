﻿using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record StatementReturnSyntax: StatementSyntax
{
    public StatementReturnSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementReturn(this);
}
