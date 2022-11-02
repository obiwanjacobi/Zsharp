using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Root Syntax Node.
/// </summary>
public sealed class CompilationUnitSyntax : SyntaxNode
{
    public CompilationUnitSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.CompilationUnit;

    public ModuleSyntax? Module
        => ChildNodes.OfType<ModuleSyntax>().SingleOrDefault();

    /// <summary>
    /// Filtered collection of use import entries.
    /// </summary>
    public IEnumerable<UseImportSyntax> UseImports
        => ChildNodes.OfType<UseImportSyntax>();

    /// <summary>
    /// Filtered collection of public export entries.
    /// </summary>
    public IEnumerable<PublicExportSyntax> PublicExports
        => ChildNodes.OfType<PublicExportSyntax>();

    /// <summary>
    /// Filtered collection of member (function, type and variable) declarations.
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Members
        => ChildNodes.OfType<MemberDeclarationSyntax>();

    /// <summary>
    /// Filtered collection of statements.
    /// </summary>
    public IEnumerable<StatementSyntax> Statements
        => ChildNodes.OfType<StatementSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCompilationUnit(this);
}