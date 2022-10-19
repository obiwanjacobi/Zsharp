using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record ExpressionInvocationSyntax : ExpressionSyntax
{
    public ExpressionInvocationSyntax(string text)
        : base(text)
    { }

    public NameSyntax Identifier
        => Children.OfType<NameSyntax>().Single();

    public IEnumerable<ArgumentSyntax> Arguments
        => Children.OfType<ArgumentSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionInvocation(this);
}