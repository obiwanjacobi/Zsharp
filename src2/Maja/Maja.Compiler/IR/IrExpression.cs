using Maja.Compiler.IR.Lower;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrExpression : IrNode
{
    protected IrExpression(SyntaxNode syntax, TypeSymbol type)
        : base(syntax)
    {
        Type = type;
    }

    public IrConstant? ConstantValue
        => null;

    public TypeSymbol Type { get; }
}

internal sealed class IrExpressionInvocation : IrExpression
{
    public IrExpressionInvocation(SyntaxNode syntax, TypeSymbol type)
        : base(syntax, type)
    { }
}

internal sealed class IrExpressionLiteral : IrExpression
{
    public IrExpressionLiteral(SyntaxNode syntax, TypeSymbol type, object value)
        : base(syntax, type)
    {
        Value = value;
    }

    public object Value { get; }
}

internal sealed class IrExpressionBinary : IrExpression
{
    public IrExpressionBinary(ExpressionBinarySyntax syntax,
        IrExpression left, IrBinaryOperator op, IrExpression right)
        : base(syntax, op.TargetType)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public IrExpression Left { get; }
    public IrBinaryOperator Op { get; }
    public IrExpression Right { get; }
}