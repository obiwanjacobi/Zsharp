﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.IR.Lower;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrExpression : IrNode
{
    protected IrExpression(ExpressionSyntax syntax, TypeSymbol typeSymbol, IrConstant? constantValue = null)
        : base(syntax)
    {
        TypeSymbol = typeSymbol;
        ConstantValue = constantValue;
    }

    public IrConstant? ConstantValue { get; }

    public TypeSymbol TypeSymbol { get; }
    public TypeInferredSymbol? TypeInferredSymbol
        => TypeSymbol as TypeInferredSymbol;

    public new ExpressionSyntax Syntax
        => (ExpressionSyntax)base.Syntax;
}

internal sealed class IrExpressionInvocation : IrExpression
{
    public IrExpressionInvocation(ExpressionInvocationSyntax syntax,
        FunctionSymbol symbol, IEnumerable<IrArgument> arguments, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
        Arguments = arguments.ToImmutableArray();
    }

    public new ExpressionInvocationSyntax Syntax
        => (ExpressionInvocationSyntax)base.Syntax;

    public FunctionSymbol Symbol { get; }
    public ImmutableArray<IrArgument> Arguments { get; }
}

internal sealed class IrExpressionLiteral : IrExpression
{
    public IrExpressionLiteral(ExpressionSyntax syntax, TypeSymbol type, object value)
        : base(syntax, type, new IrConstant(value))
    { }
}

internal sealed class IrExpressionBinary : IrExpression
{
    public IrExpressionBinary(ExpressionBinarySyntax syntax,
        IrExpression left, IrBinaryOperator op, IrExpression right)
        : base(syntax, op.TargetType, IrConstant.Fold(left, op, right))
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public IrExpression Left { get; }
    public IrBinaryOperator Operator { get; }
    public IrExpression Right { get; }

    public new ExpressionBinarySyntax Syntax
        => (ExpressionBinarySyntax)base.Syntax;
}