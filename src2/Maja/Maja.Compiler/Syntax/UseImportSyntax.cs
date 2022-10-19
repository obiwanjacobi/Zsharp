using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record UseImportSyntax : SyntaxNode
{
    public UseImportSyntax(string text)
        : base(text)
    { }

    public QualifiedNameSyntax QualifiedName
        => Children.OfType<QualifiedNameSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnUseImport(this);
}
