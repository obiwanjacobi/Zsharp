﻿using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for type parameters.
/// </summary>
public abstract record TypeParameterSyntax : SyntaxNode
{
    protected TypeParameterSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name of the type parameter.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type of the parameter, if specified.
    /// </summary>
    public TypeSyntax? Type
        => ChildNodes.OfType<TypeSyntax>().SingleOrDefault();
}

/// <summary>
/// A generic type parameter (.NET).
/// </summary>
public sealed record TypeParameterGenericSyntax : TypeParameterSyntax
{
    public TypeParameterGenericSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterGeneric(this);
}

/// <summary>
/// A template type parameter (#)
/// </summary>
public sealed record TypeParameterTemplateSyntax : TypeParameterSyntax
{
    public TypeParameterTemplateSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterTemplate(this);
}

/// <summary>
/// A scalar value type parameter.
/// </summary>
public sealed record TypeParameterValueSyntax : TypeParameterSyntax
{
    public TypeParameterValueSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterValue(this);
}
