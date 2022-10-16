using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record TypeDeclarationSyntax : MemberDeclarationSyntax
{
    public TypeDeclarationSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();

    public IEnumerable<TypeParameterSyntax> Parameters
        => Children.OfType<TypeParameterSyntax>();

    public TypeMemberListSyntax<MemberEnumSyntax> Enums
        => Children.OfType<TypeMemberListSyntax<MemberEnumSyntax>>().Single();

    public TypeMemberListSyntax<MemberFieldSyntax> Fields
        => Children.OfType<TypeMemberListSyntax<MemberFieldSyntax>>().Single();

    public TypeMemberListSyntax<MemberRuleSyntax> Rules
        => Children.OfType<TypeMemberListSyntax<MemberRuleSyntax>>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnTypeDeclaration(this);
}
