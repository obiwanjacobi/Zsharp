using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for members of a type declaration.
/// </summary>
public abstract record TypeMemberSyntax : SyntaxNode
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
public sealed record MemberEnumSyntax : TypeMemberSyntax
{
    public MemberEnumSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The enum value expression, if specified.
    /// </summary>
    public ExpressionConstantSyntax? Expression
        => ChildNodes.OfType<ExpressionConstantSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberEnum(this);
}

/// <summary>
/// Represent a type field member.
/// </summary>
public sealed record MemberFieldSyntax : TypeMemberSyntax
{
    public MemberFieldSyntax(string text)
        : base(text)
    { }

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

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberField(this);
}

/// <summary>
/// Represents a validation rule of a type declaration.
/// </summary>
public sealed record MemberRuleSyntax : TypeMemberSyntax
{
    public MemberRuleSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberRule(this);
}

/// <summary>
/// A typed list of type members.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record TypeMemberListSyntax<T> : SyntaxNode
    where T : TypeMemberSyntax
{
    public TypeMemberListSyntax(string text)
        : base(text)
    { }

    public IEnumerable<T> Members
        => ChildNodes.Cast<T>();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
    {
        var result = visitor.Default;

        foreach (var member in ChildNodes)
        {
            var r = member.Accept(visitor);
            result = visitor.AggregateResult(result, r);
        }

        return result;
    }
}

