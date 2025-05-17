using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Part of an expression that accesses a member of an object.
/// </summary>
public sealed class ExpressionMemberAccessSyntax : ExpressionSyntax, ICreateSyntaxNode<ExpressionMemberAccessSyntax>
{
    public ExpressionMemberAccessSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.MemberAccess;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeMemberAccess(this);

    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    public ExpressionSyntax Left
        => LeftAs<ExpressionSyntax>();

    public T LeftAs<T>()
        where T : ExpressionSyntax
        => ChildNodes.OfType<T>().Single();

    public static ExpressionMemberAccessSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
