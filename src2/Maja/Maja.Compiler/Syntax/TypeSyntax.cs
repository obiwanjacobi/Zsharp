using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record TypeSyntax: SyntaxNode
{
    public TypeSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public IEnumerable<TypeArgumentSyntax> Arguments
        => Children.OfType<TypeArgumentSyntax>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnType(this);
}
