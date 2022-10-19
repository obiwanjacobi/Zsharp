using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record StatementExpressionSyntax : StatementSyntax
{
    public StatementExpressionSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementExpression(this);
}
