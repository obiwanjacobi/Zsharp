using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents the ret statement.
/// </summary>
public sealed class StatementReturnSyntax : StatementSyntax, ICreateSyntaxNode<StatementReturnSyntax>
{
    private StatementReturnSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementReturn;

    /// <summary>
    /// The expression of the return value, if any.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementReturn(this);

    public static StatementReturnSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
