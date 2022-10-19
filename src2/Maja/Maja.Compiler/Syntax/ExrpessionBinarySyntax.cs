using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression with an infix operator.
/// </summary>
public sealed record ExpressionBinarySyntax : ExpressionSyntax
{
    public ExpressionBinarySyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The left part of the expression.
    /// </summary>
    public ExpressionSyntax Left
        => Children.OfType<ExpressionSyntax>().First();

    /// <summary>
    /// The expression operator.
    /// </summary>
    public ExpressionOperatorSyntax Operator
        => Children.OfType<ExpressionOperatorSyntax>().Single();

    /// <summary>
    /// The right part of the expression.
    /// </summary>
    public ExpressionSyntax Right
        => Children.OfType<ExpressionSyntax>().Skip(1).Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionBinary(this);
}
