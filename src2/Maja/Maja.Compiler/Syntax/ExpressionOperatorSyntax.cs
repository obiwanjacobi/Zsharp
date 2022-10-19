namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents an expression operator.
/// </summary>
public sealed record ExpressionOperatorSyntax : SyntaxNode
{
    public ExpressionOperatorSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionOperator(this);
}
