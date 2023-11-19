using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a public export entry.
/// </summary>
public sealed class PublicExportSyntax : SyntaxNode, ICreateSyntaxNode<PublicExportSyntax>
{
    private PublicExportSyntax(string text)
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
    public static PublicExportSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new PublicExportSyntax(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}