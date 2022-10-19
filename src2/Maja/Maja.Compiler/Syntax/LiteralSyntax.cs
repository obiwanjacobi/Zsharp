namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a literal numerical value.
/// </summary>
public sealed record LiteralNumberSyntax : SyntaxNode
{
    public LiteralNumberSyntax(string text)
        : base(text)
    { }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralNumber(this);
}

/// <summary>
/// Represents a literal string value.
/// </summary>
public sealed record LiteralStringSyntax : SyntaxNode
{
    public LiteralStringSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralString(this);
}
