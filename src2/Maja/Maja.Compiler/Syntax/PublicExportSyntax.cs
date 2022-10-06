using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record PublicExportSyntax : SyntaxNode
{
    public IEnumerable<QualifiedNameSyntax> QualifiedNames
        => Children.OfType<QualifiedNameSyntax>();
}