﻿using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a function invocation.
/// </summary>
public sealed class ExpressionInvocationSyntax : ExpressionSyntax
{
    public ExpressionInvocationSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.InvocationExpression;

    /// <summary>
    /// The name of the function being invoked.
    /// </summary>
    public ExpressionIdentifierSyntax Identifier
        => ChildNodes.OfType<ExpressionIdentifierSyntax>().Single();

    /// <summary>
    /// The list of function arguments, if any.
    /// </summary>
    public IEnumerable<ArgumentSyntax> Arguments
        => ChildNodes.OfType<ArgumentSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnExpressionInvocation(this);
}