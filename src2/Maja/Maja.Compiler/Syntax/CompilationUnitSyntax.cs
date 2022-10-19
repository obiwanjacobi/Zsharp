using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Root Syntax Node.
/// </summary>
public sealed record CompilationUnitSyntax : SyntaxNode
{
    public CompilationUnitSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// Filtered collection of use import entries.
    /// </summary>
    public IEnumerable<UseImportSyntax> UseImports
        => Children.OfType<UseImportSyntax>();

    /// <summary>
    /// Filtered collection of pubilc export entries.
    /// </summary>
    public IEnumerable<PublicExportSyntax> PublicExports
        => Children.OfType<PublicExportSyntax>();

    /// <summary>
    /// Filtered collection of member (function, type and variable) declarations.
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Members
        => Children.OfType<MemberDeclarationSyntax>();

    /// <summary>
    /// Filtered collection of statements.
    /// </summary>
    public IEnumerable<StatementSyntax> Statements
        => Children.OfType<StatementSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCompilationUnit(this);
}