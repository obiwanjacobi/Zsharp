using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A symbol name or identifier used in an expression.
/// </summary>
public sealed class ExpressionIdentifierSyntax : ExpressionSyntax, ICreateSyntaxNode<ExpressionIdentifierSyntax>
{
    private ExpressionIdentifierSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    public override SyntaxKind SyntaxKind
        => SyntaxKind.IdentifierExpression;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionIdentifier(this);

    public static ExpressionIdentifierSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
    => new(text)
    {
        Location = location,
        Children = children,
        ChildNodes = childNodes,
        TrailingTokens = trailingTokens
    };
}