namespace Maja.Compiler.Syntax;

public record ExpressionSyntax : SyntaxNode
{
    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpression(this);
}
