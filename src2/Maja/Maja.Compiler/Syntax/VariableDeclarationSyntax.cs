using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record VariableDeclarationSyntax : MemberDeclarationSyntax
{
    public VariableDeclarationSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclaration(this);
}

public sealed record VariableDeclarationTypedSyntax : VariableDeclarationSyntax
{
    public VariableDeclarationTypedSyntax(string text)
        : base(text)
    { }

    public TypeSyntax? Type
        => Children.OfType<TypeSyntax>().SingleOrDefault();
}

public sealed record VariableDeclarationInferredSyntax : VariableDeclarationSyntax
{
    public VariableDeclarationInferredSyntax(string text)
        : base(text)
    { }
}