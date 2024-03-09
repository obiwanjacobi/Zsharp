using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a type declaration.
/// </summary>
public sealed class TypeDeclarationSyntax : MemberDeclarationSyntax, ICreateSyntaxNode<TypeDeclarationSyntax>
{
    private TypeDeclarationSyntax(string text)
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
    /// Optional base type.
    /// </summary>
    public TypeSyntax? BaseType
        => ChildNodes.OfType<TypeSyntax>().SingleOrDefault();

    /// <summary>
    /// The type parameters, if any.
    /// </summary>
    public IEnumerable<TypeParameterSyntax> TypeParameters
        => ChildNodes.OfType<TypeParameterSyntax>();

    /// <summary>
    /// The enumeration list.
    /// </summary>
    public TypeMemberListSyntax<MemberEnumSyntax>? Enums
        => ChildNodes.OfType<TypeMemberListSyntax<MemberEnumSyntax>>().SingleOrDefault();

    /// <summary>
    /// The field list.
    /// </summary>
    public TypeMemberListSyntax<MemberFieldSyntax>? Fields
        => ChildNodes.OfType<TypeMemberListSyntax<MemberFieldSyntax>>().SingleOrDefault();

    /// <summary>
    /// The rule list.
    /// </summary>
    public TypeMemberListSyntax<MemberRuleSyntax>? Rules
        => ChildNodes.OfType<TypeMemberListSyntax<MemberRuleSyntax>>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeDeclaration(this);
    public static TypeDeclarationSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
