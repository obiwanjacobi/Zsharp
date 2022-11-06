using System.Collections.Generic;
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
    /// The qualified names of the imports.
    /// </summary>
    public IEnumerable<QualifiedNameSyntax> QualifiedNames
        => ChildNodes.OfType<QualifiedNameSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnUseImport(this);
}
