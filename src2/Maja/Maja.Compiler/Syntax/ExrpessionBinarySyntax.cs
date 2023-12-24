using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An expression with an infix operator.
/// </summary>
public sealed class ExpressionBinarySyntax : ExpressionSyntax, ICreateSyntaxNode<ExpressionBinarySyntax>
{
    private ExpressionBinarySyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.BinaryExpression;

    public override bool IsPrecedenceValid
        => CheckForPrecedence().precedenceValid;

    // precedence is valid when all binary expressions are wrapped in ()
    // or when one operator (kind) is not, but the others are.
    private (bool precedenceValid, ExpressionOperatorKind commonOperator) CheckForPrecedence()
    {
        var thisOperator = Operator.OperatorKind;
        var thisPrecedence = Precedence;

        var left = Left as ExpressionBinarySyntax;
        if (left is not null)
        {
            var (leftPrecedence, leftOperator) = left.CheckForPrecedence();
            if (!leftPrecedence)
                return (false, leftOperator);

            if (leftOperator != thisOperator &&
                leftOperator != ExpressionOperatorKind.Unknown &&
                !thisPrecedence)
                return (false, thisOperator);
        }

        var right = Right as ExpressionBinarySyntax;
        if (right is not null)
        {
            var (rightPrecedence, rightOperator) = right.CheckForPrecedence();
            if (!rightPrecedence)
                return (false, rightOperator);

            if (rightOperator != thisOperator &&
                rightOperator != ExpressionOperatorKind.Unknown &&
                !thisPrecedence)
                return (false, thisOperator);
        }

        return (true, thisPrecedence ? ExpressionOperatorKind.Unknown : thisOperator);
    }

    /// <summary>
    /// The left part of the expression.
    /// </summary>
    public ExpressionSyntax Left
        => ChildNodes.OfType<ExpressionSyntax>().First();

    /// <summary>
    /// The expression operator.
    /// </summary>
    public ExpressionOperatorSyntax Operator
        => ChildNodes.OfType<ExpressionOperatorSyntax>().Single();

    /// <summary>
    /// The right part of the expression.
    /// </summary>
    public ExpressionSyntax Right
        => ChildNodes.OfType<ExpressionSyntax>().Skip(1).Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionBinary(this);

    public static ExpressionBinarySyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
