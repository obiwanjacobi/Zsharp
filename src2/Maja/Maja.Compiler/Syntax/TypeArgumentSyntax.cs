﻿using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a type argument.
/// </summary>
public sealed record TypeArgumentSyntax : SyntaxNode
{
    public TypeArgumentSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The type specified as the argument.
    /// </summary>
    public TypeSyntax? Type
        => ChildNodes.OfType<TypeSyntax>().SingleOrDefault();

    /// <summary>
    /// The 'type' value expression as the argument.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeArgument(this);
}