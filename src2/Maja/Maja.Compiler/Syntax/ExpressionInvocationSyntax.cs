using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a function invocation.
/// </summary>
public sealed record ExpressionInvocationSyntax : ExpressionSyntax
{
    public ExpressionInvocationSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name of the function being invoced.
    /// </summary>
    public NameSyntax Identifier
        => Children.OfType<NameSyntax>().Single();

    /// <summary>
    /// The list of function arguments, if any.
    /// </summary>
    public IEnumerable<ArgumentSyntax> Arguments
        => Children.OfType<ArgumentSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionInvocation(this);
}