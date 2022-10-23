using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a public export entry.
/// </summary>
public sealed record PublicExportSyntax : SyntaxNode
{
    public PublicExportSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.PublicExportDirective;

    /// <summary>
    /// The qualified names specified at the export statement.
    /// </summary>
    public IEnumerable<QualifiedNameSyntax> QualifiedNames
        => ChildNodes.OfType<QualifiedNameSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnPublicExport(this);
}