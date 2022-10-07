namespace Maja.Compiler.Syntax;

public sealed record LiteralNumberSyntax : SyntaxNode
{
    public LiteralNumberSyntax(string value)
        => Value = value;

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralNumber(this);
}

public sealed record LiteralStringSyntax : SyntaxNode
{
    public LiteralStringSyntax(string value)
        => Value = value;

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralString(this);
}
