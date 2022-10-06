using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record UseImportSyntax : SyntaxNode
{
    public QualifiedNameSyntax QualifiedName
        => Children.OfType<QualifiedNameSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnUseImport(this);
}
