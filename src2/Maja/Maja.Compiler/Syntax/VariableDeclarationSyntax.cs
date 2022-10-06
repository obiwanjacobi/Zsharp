using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record VariableDeclarationSyntax : MemberDeclarationSyntax
{
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();
}
