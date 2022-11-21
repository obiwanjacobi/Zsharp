using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression with a prefix operator.
/// </summary>
public sealed class ExpressionUnarySyntax : ExpressionSyntax
{
    public ExpressionUnarySyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.UnaryExpression;

    public override bool IsPrecedenceValid
        => Operand.IsPrecedenceValid;

    /// <summary>
    /// The operand of the expression.
    /// </summary>
    public ExpressionSyntax Operand
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    /// <summary>
    /// The expression operator.
    /// </summary>
    public ExpressionOperatorSyntax Operator
        => ChildNodes.OfType<ExpressionOperatorSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionUnary(this);
}
