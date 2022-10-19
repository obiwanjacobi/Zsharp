using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// An argument for a function parameter.
/// </summary>
public sealed record ArgumentSyntax : SyntaxNode
{
    public ArgumentSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The expression that represent the argument.
    /// </summary>
    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnArgument(this);
}