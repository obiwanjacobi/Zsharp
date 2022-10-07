namespace Maja.Compiler.Syntax;

public record ExpressionSyntax : SyntaxNode
{
    public ExpressionSyntax(bool precedence = false)
        => Precedence = precedence;

    /// <summary>
    /// Indication that '()' was around this expression
    /// </summary>
    public bool Precedence { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpression(this);
}
