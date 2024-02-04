using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression who's value can be determined at compile time.
/// </summary>
public abstract class ExpressionConstantSyntax : ExpressionSyntax
{
    protected ExpressionConstantSyntax(string text)
        : base(text)
    { }
}

/// <summary>
/// An expression that represents a literal value.
/// </summary>
public sealed class ExpressionLiteralSyntax : ExpressionConstantSyntax, ICreateSyntaxNode<ExpressionLiteralSyntax>
{
    private ExpressionLiteralSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralExpression;

    /// <summary>
    /// Set when the literal expression represents a number value.
    /// </summary>
    public LiteralNumberSyntax? LiteralNumber
        => ChildNodes.OfType<LiteralNumberSyntax>().SingleOrDefault();

    /// <summary>
    /// Set when the literal expression represents a string value.
    /// </summary>
    public LiteralStringSyntax? LiteralString
        => ChildNodes.OfType<LiteralStringSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteral(this);

    public static ExpressionLiteralSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// An expression that represents a literal boolean value.
/// </summary>
public sealed class ExpressionLiteralBoolSyntax : ExpressionConstantSyntax, ICreateSyntaxNode<ExpressionLiteralBoolSyntax>
{
    private ExpressionLiteralBoolSyntax(string text)
        : base(text)
    {
        Value = text == "true";
    }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralBoolExpression;

    /// <summary>
    /// The parsed boolean value.
    /// </summary>
    public bool Value { get; }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteralBool(this);

    public static ExpressionLiteralBoolSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}