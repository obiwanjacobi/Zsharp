using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record TypeSyntax: SyntaxNode
{
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnType(this);
}
