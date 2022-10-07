namespace Maja.Compiler.Syntax;

public sealed record ExpressionOperatorSyntax : SyntaxNode
{
    public ExpressionOperatorSyntax(string text)
        : base(text)
    { }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionOperator(this);
}
