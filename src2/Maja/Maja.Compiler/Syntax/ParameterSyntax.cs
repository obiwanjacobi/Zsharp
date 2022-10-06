using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record ParameterSyntax: SyntaxNode
{
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public TypeSyntax Type
        => Children.OfType<TypeSyntax>().Single();
}
