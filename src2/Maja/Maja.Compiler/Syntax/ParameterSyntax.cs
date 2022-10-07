﻿using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record ParameterSyntax: SyntaxNode
{
    public ParameterSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public TypeSyntax Type
        => Children.OfType<TypeSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnParameter(this);
}
