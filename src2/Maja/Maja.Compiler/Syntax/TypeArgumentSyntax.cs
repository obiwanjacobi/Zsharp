using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a type argument.
/// </summary>
public sealed class TypeArgumentSyntax : SyntaxNode, ICreateSyntaxNode<TypeArgumentSyntax>
{
    private TypeArgumentSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypeArgument;

    /// <summary>
    /// The type specified as the argument.
    /// </summary>
    public TypeSyntax? Type
        => ChildNodes.OfType<TypeSyntax>().SingleOrDefault();

    /// <summary>
    /// The 'type' value expression as the argument.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeArgument(this);
    public static TypeArgumentSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}