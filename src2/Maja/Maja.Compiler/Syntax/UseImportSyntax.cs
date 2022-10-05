using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record UseImportSyntax : SyntaxNode
{
    public QualifiedNameSyntax QualifiedName
    {
        get
        {
            var name = Children.OfType<QualifiedNameSyntax>().SingleOrDefault();
            Debug.Assert(name is not null);
            return name!;
        }
    }
}