using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal abstract class IrExpression : IrNode
{
    protected IrExpression(TypeSymbol typeSymbol, IrConstant? constantValue = null)
    {
        TypeSymbol = typeSymbol;
        ConstantValue = constantValue;
    }
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

internal sealed class IrExpressionRange : IrExpression, IrContainer
{
    public IrExpressionRange(ExpressionRangeSyntax syntax, IrExpression? start, IrExpression? end)
        : base(syntax, TypeSymbol.I32)  // I32 => dotnet compatible
    {
        Start = start;
        End = end;
    }

    public IrExpression? Start { get; }
    public IrExpression? End { get; }

    public new ExpressionRangeSyntax Syntax
        => (ExpressionRangeSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Start.GetDescendantsOfType<T>()
        .Concat(End.GetDescendantsOfType<T>());
}

internal sealed class IrExpressionInvocation : IrExpression, IrContainer
{
    public IrExpressionInvocation(ExpressionInvocationSyntax syntax,
        DeclaredFunctionSymbol symbol, IEnumerable<IrTypeArgument> typeArguments, IEnumerable<IrArgument> arguments, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
        TypeArguments = typeArguments.ToImmutableArray();
        Arguments = arguments.ToImmutableArray();
    }

    public new ExpressionInvocationSyntax Syntax
        => (ExpressionInvocationSyntax)base.Syntax;

    public DeclaredFunctionSymbol Symbol { get; }
    public ImmutableArray<IrTypeArgument> TypeArguments { get; }
    public ImmutableArray<IrArgument> Arguments { get; }

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => TypeArguments.GetDescendantsOfType<T>()
        .Concat(Arguments.GetDescendantsOfType<T>());
}

internal sealed class IrExpressionTypeInitializer : IrExpression, IrContainer
{
    public IrExpressionTypeInitializer(ExpressionTypeInitializerSyntax syntax, TypeSymbol symbol, IEnumerable<IrTypeArgument> typeArguments, IEnumerable<IrTypeInitializerField> fields)
        : base(syntax, symbol)
    {
        TypeArguments = typeArguments.ToImmutableArray();
        Fields = fields.ToImmutableArray();
    }

    public ImmutableArray<IrTypeArgument> TypeArguments { get; }
    public ImmutableArray<IrTypeInitializerField> Fields { get; }

    public new ExpressionTypeInitializerSyntax Syntax
        => (ExpressionTypeInitializerSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => TypeArguments.GetDescendantsOfType<T>()
        .Concat(Fields.GetDescendantsOfType<T>());
}

internal sealed class IrExpressionLiteral : IrExpression
{
    public IrExpressionLiteral(TypeSymbol type, object value)
        : base(type, new IrConstant(value))
    { }
    public IrExpressionLiteral(ExpressionConstantSyntax syntax, TypeSymbol type, object value)
        : base(syntax, type, new IrConstant(value))
    { }

    public IrExpressionLiteral(ExpressionConstantSyntax syntax, TypeSymbol type, IrConstant value)
        : base(syntax, type, value)
    { }

    public new ExpressionConstantSyntax Syntax
        => (ExpressionConstantSyntax)base.Syntax;
}

internal sealed class IrExpressionBinary : IrExpression, IrContainer
{
    internal IrExpressionBinary(
        IrExpression left, IrOperatorBinary op, IrExpression right)
        : base(op.TargetType, IrConstant.Fold(left, op, right))
    {
        Left = left;
        Operator = op;
        Right = right;
    }
    public IrExpressionBinary(ExpressionBinarySyntax syntax,
        IrExpression left, IrOperatorBinary op, IrExpression right)
        : base(syntax, op.TargetType, IrConstant.Fold(left, op, right))
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public IrExpression Left { get; }
    public IrOperatorBinary Operator { get; }
    public IrExpression Right { get; }

    public new ExpressionBinarySyntax Syntax
        => (ExpressionBinarySyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Left.GetDescendantsOfType<T>()
        .Concat(Operator.GetDescendantsOfType<T>())
        .Concat(Right.GetDescendantsOfType<T>());
}