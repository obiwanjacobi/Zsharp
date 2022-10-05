using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record PublicExportSyntax : SyntaxNode
{
    public IEnumerable<QualifiedNameSyntax> Names
        => Children.OfType<QualifiedNameSyntax>();
}