﻿using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a parameter in a function declaration.
/// </summary>
public sealed class ParameterSyntax : SyntaxNode
{
    public ParameterSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.FunctionParameter;

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnParameter(this);
}
