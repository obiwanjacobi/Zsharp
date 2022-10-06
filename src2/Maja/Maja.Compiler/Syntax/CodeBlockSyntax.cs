using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record CodeBlockSyntax: SyntaxNode
{
    public IEnumerable<MemberDeclarationSyntax> Members
        => Children.OfType<MemberDeclarationSyntax>();

    public IEnumerable<StatementSyntax> Statements
        => Children.OfType<StatementSyntax>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnCodeBlock(this);
}
