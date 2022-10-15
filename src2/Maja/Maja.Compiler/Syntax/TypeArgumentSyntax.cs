using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record TypeArgumentSyntax : SyntaxNode
{
    public TypeArgumentSyntax(string text)
        : base(text)
    { }

    public TypeSyntax? Type
        => Children.OfType<TypeSyntax>().SingleOrDefault();

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeArgument(this);
}