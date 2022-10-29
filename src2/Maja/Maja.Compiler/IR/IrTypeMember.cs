using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrTypeMember : IrNode
{
    protected IrTypeMember(TypeMemberSyntax syntax)
        : base(syntax)
    { }

    public new TypeMemberSyntax Syntax
        => (TypeMemberSyntax)base.Syntax;
}

internal sealed class IrTypeMemberEnum : IrTypeMember
{
    public IrTypeMemberEnum(MemberEnumSyntax syntax, EnumSymbol symbol, IrExpression? expr, object value)
        : base(syntax)
    {
        Symbol = symbol;
        ValueExpression = expr;
        Value = value;
    }

    public EnumSymbol Symbol { get; }
    public IrExpression? ValueExpression { get; }
    public object Value { get; }

    public new MemberEnumSyntax Syntax
        => (MemberEnumSyntax)base.Syntax;
}

internal sealed class IrTypeMemberField : IrTypeMember
{
    public IrTypeMemberField(MemberFieldSyntax syntax, FieldSymbol symbol, IrType type, IrExpression? defaultValue)
        : base(syntax)
    {
        Symbol = symbol;
        Type = type;
        DefaultValue = defaultValue;
    }

    public FieldSymbol Symbol { get; }
    public IrType Type { get; }
    public IrExpression? DefaultValue { get; }

    public new MemberFieldSyntax Syntax
        => (MemberFieldSyntax)base.Syntax;
}

internal sealed class IrTypeMemberRule : IrTypeMember
{
    public IrTypeMemberRule(MemberRuleSyntax syntax, RuleSymbol symbol, IrExpression expression)
        : base(syntax)
    {
        Symbol = symbol;
        Expression = expression;
    }

    public RuleSymbol Symbol { get; }
    public IrExpression Expression { get; }

    public new MemberRuleSyntax Syntax
        => (MemberRuleSyntax)base.Syntax;
}
