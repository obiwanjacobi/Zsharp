using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record CompilationUnitSyntax : SyntaxNode
{
    public CompilationUnitSyntax(string text)
        : base(text)
    { }

    public IEnumerable<UseImportSyntax> UseImports
        => Children.OfType<UseImportSyntax>();

    public IEnumerable<PublicExportSyntax> PublicExports
        => Children.OfType<PublicExportSyntax>();

    public IEnumerable<MemberDeclarationSyntax> Members
        => Children.OfType<MemberDeclarationSyntax>();

    public IEnumerable<StatementSyntax> Statements
        => Children.OfType<StatementSyntax>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCompilationUnit(this);
}