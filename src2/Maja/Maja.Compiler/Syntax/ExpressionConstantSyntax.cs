using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression who's value can be determined at compile time.
/// </summary>
public abstract record ExpressionConstantSyntax : ExpressionSyntax
{
    protected ExpressionConstantSyntax(string text)
        : base(text)
    { }
}

/// <summary>
/// An expression that represents a literal value.
/// </summary>
public record ExpressionLiteralSyntax : ExpressionConstantSyntax
{
    public ExpressionLiteralSyntax(string text)
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
}

/// <summary>
/// An expression that represents a literal boolean value.
/// </summary>
public record ExpressionLiteralBoolSyntax : ExpressionConstantSyntax
{
    public ExpressionLiteralBoolSyntax(string text)
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
}