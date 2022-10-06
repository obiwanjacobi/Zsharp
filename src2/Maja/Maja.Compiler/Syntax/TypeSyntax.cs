using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record TypeSyntax: SyntaxNode
{
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();
}
