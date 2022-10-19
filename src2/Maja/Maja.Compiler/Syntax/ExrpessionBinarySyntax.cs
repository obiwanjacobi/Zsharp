using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record ExpressionBinarySyntax : ExpressionSyntax
{
    public ExpressionBinarySyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax Left
        => Children.OfType<ExpressionSyntax>().First();

    public ExpressionOperatorSyntax Operator
        => Children.OfType<ExpressionOperatorSyntax>().Single();

    public ExpressionSyntax Right
        => Children.OfType<ExpressionSyntax>().Skip(1).Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionBinary(this);
}
