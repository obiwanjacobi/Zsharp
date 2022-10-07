using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record VariableDeclarationSyntax : MemberDeclarationSyntax
{
    public VariableDeclarationSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public TypeSyntax? Type
        => Children.OfType<TypeSyntax>().SingleOrDefault();

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnVariableDeclaration(this);
}
