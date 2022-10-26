namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a literal numerical value.
/// </summary>
public sealed record LiteralNumberSyntax : ExpressionSyntax
{
    public LiteralNumberSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralNumber;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralNumber(this);
}

/// <summary>
/// Represents a literal string value.
/// </summary>
public sealed record LiteralStringSyntax : ExpressionSyntax
{
    public LiteralStringSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralString;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralString(this);
}
