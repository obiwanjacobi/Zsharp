﻿using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// The loop keyword with it's (optional) properties.
/// </summary>
public sealed class StatementLoopSyntax : StatementSyntax, ICreateSyntaxNode<StatementLoopSyntax>
{
    public StatementLoopSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public CodeBlockSyntax CodeBlock
        => ChildNodes.OfType<CodeBlockSyntax>().Single();

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementLoop;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementLoop(this);

    public static StatementLoopSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
