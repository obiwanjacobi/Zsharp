using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a use import statement.
/// </summary>
public sealed class UseImportSyntax : SyntaxNode, ICreateSyntaxNode<UseImportSyntax>
{
    private UseImportSyntax(string text)
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
    public static UseImportSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
