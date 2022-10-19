using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a type declaration.
/// </summary>
public sealed record TypeDeclarationSyntax : MemberDeclarationSyntax
{
    public TypeDeclarationSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name of the type.
    /// </summary>
    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type parameters, if any.
    /// </summary>
    public IEnumerable<TypeParameterSyntax> TypeParameters
        => Children.OfType<TypeParameterSyntax>();

    /// <summary>
    /// The enumeration list.
    /// </summary>
    public TypeMemberListSyntax<MemberEnumSyntax> Enums
        => Children.OfType<TypeMemberListSyntax<MemberEnumSyntax>>().Single();

    /// <summary>
    /// The field list.
    /// </summary>
    public TypeMemberListSyntax<MemberFieldSyntax> Fields
        => Children.OfType<TypeMemberListSyntax<MemberFieldSyntax>>().Single();

    /// <summary>
    /// The rule list.
    /// </summary>
    public TypeMemberListSyntax<MemberRuleSyntax> Rules
        => Children.OfType<TypeMemberListSyntax<MemberRuleSyntax>>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeDeclaration(this);
}
