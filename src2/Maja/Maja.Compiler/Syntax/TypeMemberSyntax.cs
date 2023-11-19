using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for members of a type declaration.
/// </summary>
public abstract class TypeMemberSyntax : SyntaxNode
{
    public TypeMemberSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The name or identifier of the member.
    /// </summary>
    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();
}

/// <summary>
/// Represents a type enum member.
/// </summary>
public sealed class MemberEnumSyntax : TypeMemberSyntax, ICreateSyntaxNode<MemberEnumSyntax>
{
    private MemberEnumSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.MemberEnum;

    /// <summary>
    /// The enum value expression, if specified.
    /// </summary>
    public ExpressionConstantSyntax? Expression
        => ChildNodes.OfType<ExpressionConstantSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberEnum(this);
    
    public static MemberEnumSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represent a type field member.
/// </summary>
public sealed class MemberFieldSyntax : TypeMemberSyntax, ICreateSyntaxNode<MemberFieldSyntax>
{
    private MemberFieldSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.MemberField;

    /// <summary>
    /// The type of the field.
    /// </summary>
    public TypeSyntax Type
        => ChildNodes.OfType<TypeSyntax>().Single();

    /// <summary>
    /// The field initialization expression.
    /// </summary>
    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberField(this);
    public static MemberFieldSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represents a validation rule of a type declaration.
/// </summary>
public sealed class MemberRuleSyntax : TypeMemberSyntax, ICreateSyntaxNode<MemberRuleSyntax>
{
    private MemberRuleSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.MemberRule;

    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberRule(this);
    public static MemberRuleSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// A typed list of type members.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class TypeMemberListSyntax<T> : SyntaxNode, ICreateSyntaxNode<TypeMemberListSyntax<T>>
    where T : TypeMemberSyntax
{
    private TypeMemberListSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.MemberList;

    public IEnumerable<T> Items
        => ChildNodes.Cast<T>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
    {
        var result = visitor.Default;

        foreach (var member in ChildNodes)
        {
            var r = member.Accept(visitor);
            result = visitor.AggregateResult(result, r);
        }

        return result;
    }

    public static TypeMemberListSyntax<T> Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

