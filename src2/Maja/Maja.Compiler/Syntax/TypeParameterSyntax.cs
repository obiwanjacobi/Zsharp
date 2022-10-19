using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record TypeParameterSyntax : SyntaxNode
{
    protected TypeParameterSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public TypeSyntax? Type
        => Children.OfType<TypeSyntax>().SingleOrDefault();
}

public sealed record TypeParameterGenericSyntax : TypeParameterSyntax
{
    public TypeParameterGenericSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterGeneric(this);
}

public sealed record TypeParameterTemplateSyntax : TypeParameterSyntax
{
    public TypeParameterTemplateSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterTemplate(this);
}

public sealed record TypeParameterValueSyntax : TypeParameterSyntax
{
    public TypeParameterValueSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeParameterValue(this);
}

