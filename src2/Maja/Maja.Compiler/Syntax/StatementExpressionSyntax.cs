﻿using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represent the case where an expression is used as a statement.
/// </summary>
public sealed class StatementExpressionSyntax : StatementSyntax, ICreateSyntaxNode<StatementExpressionSyntax>
{
    private StatementExpressionSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementExpression;

    /// <summary>
    /// The expression of this statement.
    /// </summary>
    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementExpression(this);

    public static StatementExpressionSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
