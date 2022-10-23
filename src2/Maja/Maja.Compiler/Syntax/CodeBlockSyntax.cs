using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A block of code typically representing a scope.
/// </summary>
public sealed record CodeBlockSyntax : SyntaxNode
{
    public CodeBlockSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.CodeBlock;

    /// <summary>
    /// Filtered collection of only member (function, type and variable) declarations.
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Members
        => ChildNodes.OfType<MemberDeclarationSyntax>();

    /// <summary>
    /// Filtered collection of only statements.
    /// </summary>
    public IEnumerable<StatementSyntax> Statements
        => ChildNodes.OfType<StatementSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCodeBlock(this);
}
