namespace Maja.Compiler.Syntax;

public record ExpressionSyntax : SyntaxNode
{
    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpression(this);
}

public record ExpressionConstSyntax : ExpressionSyntax
{

}

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