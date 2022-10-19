using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record ArgumentSyntax : SyntaxNode
{
    public ArgumentSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnArgument(this);
}