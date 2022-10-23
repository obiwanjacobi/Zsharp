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

    public override SyntaxKind SyntaxKind
        => SyntaxKind.TypeDeclaration;

    /// <summary>
    /// The name of the type.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    /// <summary>
    /// The type parameters, if any.
    /// </summary>
    public IEnumerable<TypeParameterSyntax> TypeParameters
        => ChildNodes.OfType<TypeParameterSyntax>();

    /// <summary>
    /// The enumeration list.
    /// </summary>
    public TypeMemberListSyntax<MemberEnumSyntax> Enums
        => ChildNodes.OfType<TypeMemberListSyntax<MemberEnumSyntax>>().Single();

    /// <summary>
    /// The field list.
    /// </summary>
    public TypeMemberListSyntax<MemberFieldSyntax> Fields
        => ChildNodes.OfType<TypeMemberListSyntax<MemberFieldSyntax>>().Single();

    /// <summary>
    /// The rule list.
    /// </summary>
    public TypeMemberListSyntax<MemberRuleSyntax> Rules
        => ChildNodes.OfType<TypeMemberListSyntax<MemberRuleSyntax>>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeDeclaration(this);
}
