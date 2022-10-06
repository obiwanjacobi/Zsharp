using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record CompilationUnitSyntax : SyntaxNode
{
    public IEnumerable<UseImportSyntax> UseImports
        => Children.OfType<UseImportSyntax>();

    public IEnumerable<PublicExportSyntax> PublicExports
        => Children.OfType<PublicExportSyntax>();

    public IEnumerable<MemberDeclarationSyntax> Members
        => Children.OfType<MemberDeclarationSyntax>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCompilationUnit(this);
}