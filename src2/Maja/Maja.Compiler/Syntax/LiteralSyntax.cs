namespace Maja.Compiler.Syntax;

public sealed record LiteralNumberSyntax : SyntaxNode
{
    public LiteralNumberSyntax(string text)
        : base(text)
    { }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralNumber(this);
}

public sealed record LiteralStringSyntax : SyntaxNode
{
    public LiteralStringSyntax(string text)
        : base(text)
    { }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralString(this);
}
