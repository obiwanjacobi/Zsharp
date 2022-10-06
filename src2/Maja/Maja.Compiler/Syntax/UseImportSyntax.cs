using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record UseImportSyntax : SyntaxNode
{
    public QualifiedNameSyntax QualifiedName
        => Children.OfType<QualifiedNameSyntax>().Single();
}
