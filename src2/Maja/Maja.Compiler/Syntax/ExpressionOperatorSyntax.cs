namespace Maja.Compiler.Syntax;

public sealed record ExpressionOperatorSyntax : SyntaxNode
{
    public ExpressionOperatorSyntax(string value)
        => Value = value;

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionOperator(this);
}
