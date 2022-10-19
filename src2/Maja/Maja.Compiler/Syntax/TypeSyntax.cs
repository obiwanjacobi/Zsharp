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

    /// <summary>
    /// The name (or alias) of the type referenced.
    /// </summary>
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type arguments, if any.
    /// </summary>
    public IEnumerable<TypeArgumentSyntax> TypeArguments
        => Children.OfType<TypeArgumentSyntax>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnType(this);
}
