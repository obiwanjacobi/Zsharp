using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.IR.Lower;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrExpression : IrNode
{
    protected IrExpression(ExpressionSyntax syntax, TypeSymbol typeSymbol)
        : base(syntax)
    {
        TypeSymbol = typeSymbol;
    }

    public IrConstant? ConstantValue
        => null;

    public TypeSymbol TypeSymbol { get; }

    public new ExpressionSyntax Syntax
        => (ExpressionSyntax)base.Syntax;
}

internal sealed class IrExpressionInvocation : IrExpression
{
    public IrExpressionInvocation(ExpressionInvocationSyntax syntax,
        FunctionSymbol? symbol, IEnumerable<IrArgument> args, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
        Args = args.ToImmutableArray();
    }

    public new ExpressionInvocationSyntax Syntax
        => (ExpressionInvocationSyntax)base.Syntax;

    public FunctionSymbol? Symbol { get; }
    public ImmutableArray<IrArgument> Args { get; }
}

internal sealed class IrExpressionLiteral : IrExpression
{
    public IrExpressionLiteral(ExpressionSyntax syntax, TypeSymbol type, object value)
        : base(syntax, type)
    {
        Value = value;
    }

    public object Value { get; }
    public new ExpressionSyntax Syntax
        => (ExpressionSyntax)base.Syntax;
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

    public new ExpressionBinarySyntax Syntax
        => (ExpressionBinarySyntax)base.Syntax;
}