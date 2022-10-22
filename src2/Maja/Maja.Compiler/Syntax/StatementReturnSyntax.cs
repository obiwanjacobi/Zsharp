using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents the ret statement.
/// </summary>
public sealed record StatementReturnSyntax : StatementSyntax
{
    public StatementReturnSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The expression of the return value, if any.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementReturn(this);
}
