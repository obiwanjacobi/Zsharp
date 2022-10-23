using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a reference to a type by name.
/// </summary>
public sealed record TypeSyntax : SyntaxNode
{
    public TypeSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.Type;

    /// <summary>
    /// The name (or alias) of the type referenced.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type arguments, if any.
    /// </summary>
    public IEnumerable<TypeArgumentSyntax> TypeArguments
        => ChildNodes.OfType<TypeArgumentSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnType(this);
}
