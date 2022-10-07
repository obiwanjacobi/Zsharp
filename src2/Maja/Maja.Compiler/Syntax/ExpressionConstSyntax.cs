namespace Maja.Compiler.Syntax;

public abstract record ExpressionConstSyntax : ExpressionSyntax
{ }

public record ExpressionLiteralSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralSyntax(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteral(this);
}

public record ExpressionLiteralBoolSyntax : ExpressionConstSyntax
{
    public ExpressionLiteralBoolSyntax(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionLiteralBool(this);
}