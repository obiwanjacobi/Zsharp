using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a use import statement.
/// </summary>
public sealed class UseImportSyntax : SyntaxNode
{
    public UseImportSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.UseImportDirective;

    /// <summary>
    /// The qualified name of the import.
    /// </summary>
    public QualifiedNameSyntax QualifiedName
        => ChildNodes.OfType<QualifiedNameSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnUseImport(this);
}
