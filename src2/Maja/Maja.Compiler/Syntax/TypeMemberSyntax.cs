using System.Collections.Generic;
using System.Linq;

namespace Maja.Compiler.Syntax;

public abstract record TypeMemberSyntax: SyntaxNode
{
    public TypeMemberSyntax(string text)
        : base(text)
    { }

    public NameSyntax Name
        => Children.OfType<NameSyntax>().Single();
}

public sealed record MemberEnumSyntax : TypeMemberSyntax
{
    public MemberEnumSyntax(string text)
        : base(text)
    { }

    public ExpressionConstantSyntax? Expression
        => Children.OfType<ExpressionConstantSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberEnum(this);
}

public sealed record MemberFieldSyntax : TypeMemberSyntax
{
    public MemberFieldSyntax(string text)
        : base(text)
    { }

    public TypeSyntax Type
        => Children.OfType<TypeSyntax>().Single();

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberField(this);
}

public sealed record MemberRuleSyntax : TypeMemberSyntax
{
    public MemberRuleSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnMemberRule(this);
}


public sealed record TypeMemberListSyntax<T> : SyntaxNode
    where T : TypeMemberSyntax
{
    public TypeMemberListSyntax(string text)
        : base(text)
    { }

    public IEnumerable<T> Members
        => Children.Cast<T>();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
    {
        var result = visitor.Default;
        
        foreach (var member in Children)
        {
            var r = member.Accept(visitor);
            result = visitor.AggregateResult(result, r);
        }

        return result;
    }
}

