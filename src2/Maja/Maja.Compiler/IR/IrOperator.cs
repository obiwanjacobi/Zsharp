using System;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal enum IrBinaryOperatorKind
{
    // arithmetic
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Power,
    Root,

    // bitwise
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor,
    BitwiseShiftLeft,
    BitwiseShiftRight,
    BitwiseShiftRightSign,
    BitwiseRollLeft,
    BitwiseRollRight,

    // comparison
    Equals,
    NotEquals,
    Greater,
    GreaterOrEquals,
    Lesser,
    LesserOrEquals,

    // logical
    And,
    Or,
    Xor,
}

internal sealed class IrBinaryOperator : IrNode
{
    public IrBinaryOperator(ExpressionOperatorSyntax syntax, TypeSymbol operandType)
        : base(syntax)
    {
        Kind = DetermineKind(syntax);
        OperandType = operandType;
        TargetType = DetermineTargeType(syntax, operandType);
    }

    private static TypeSymbol DetermineTargeType(ExpressionOperatorSyntax syntax, TypeSymbol operandType)
    {
        switch (syntax.OperatorKind)
        {
            case ExpressionOperatorKind.Equals:
            case ExpressionOperatorKind.NotEquals:
            case ExpressionOperatorKind.Greater:
            case ExpressionOperatorKind.GreaterOrEquals:
            case ExpressionOperatorKind.Lesser:
            case ExpressionOperatorKind.LesserOrEquals:
                return TypeSymbol.Bool;
            default:
                return operandType;
        }
    }

    private static IrBinaryOperatorKind DetermineKind(ExpressionOperatorSyntax syntax)
    {
        return syntax.OperatorKind switch
        {
            ExpressionOperatorKind.And => IrBinaryOperatorKind.And,
            ExpressionOperatorKind.BitAnd => IrBinaryOperatorKind.BitwiseAnd,
            ExpressionOperatorKind.BitOr => IrBinaryOperatorKind.BitwiseOr,
            ExpressionOperatorKind.BitRollLeft => IrBinaryOperatorKind.BitwiseRollLeft,
            ExpressionOperatorKind.BitRollRight => IrBinaryOperatorKind.BitwiseRollRight,
            ExpressionOperatorKind.BitShiftLeft => IrBinaryOperatorKind.BitwiseShiftLeft,
            ExpressionOperatorKind.BitShiftRight => IrBinaryOperatorKind.BitwiseShiftRight,
            ExpressionOperatorKind.BitShiftRightSign => IrBinaryOperatorKind.BitwiseShiftRightSign,
            ExpressionOperatorKind.BitXor => IrBinaryOperatorKind.BitwiseXor,
            ExpressionOperatorKind.Divide => IrBinaryOperatorKind.Divide,
            ExpressionOperatorKind.Equals => IrBinaryOperatorKind.Equals,
            ExpressionOperatorKind.Greater => IrBinaryOperatorKind.Greater,
            ExpressionOperatorKind.GreaterOrEquals => IrBinaryOperatorKind.GreaterOrEquals,
            ExpressionOperatorKind.Lesser => IrBinaryOperatorKind.Lesser,
            ExpressionOperatorKind.LesserOrEquals => IrBinaryOperatorKind.LesserOrEquals,
            ExpressionOperatorKind.Minus => IrBinaryOperatorKind.Subtract,
            ExpressionOperatorKind.Modulo => IrBinaryOperatorKind.Modulo,
            ExpressionOperatorKind.Multiply => IrBinaryOperatorKind.Multiply,
            ExpressionOperatorKind.NotEquals => IrBinaryOperatorKind.NotEquals,
            ExpressionOperatorKind.Or => IrBinaryOperatorKind.Or,
            ExpressionOperatorKind.Plus => IrBinaryOperatorKind.Add,
            ExpressionOperatorKind.Power => IrBinaryOperatorKind.Power,
            ExpressionOperatorKind.Root => IrBinaryOperatorKind.Root,
            ExpressionOperatorKind.Xor => IrBinaryOperatorKind.Xor,
            _ => throw new NotSupportedException($"IR: No support for Binary operator {syntax.OperatorKind}."),
        };
    }

    public IrBinaryOperatorKind Kind { get; }
    public TypeSymbol OperandType { get; }
    public TypeSymbol TargetType { get; }

    public new ExpressionOperatorSyntax Syntax
        => (ExpressionOperatorSyntax)base.Syntax;
}

internal enum IrUnaryOperatorKind
{
    Identity,   // plus does nothing
    Negate,     // arithmetic
    Invert,     // bitwise
    Not,        // logic
}

internal sealed class IrUnaryOperator : IrNode
{
    public IrUnaryOperator(ExpressionOperatorSyntax syntax, TypeSymbol operandType)
        : base(syntax)
    {
        Kind = DetermineKind(syntax);
        OperandType = operandType;
    }

    private static IrUnaryOperatorKind DetermineKind(ExpressionOperatorSyntax syntax)
    {
        return syntax.OperatorKind switch
        {
            ExpressionOperatorKind.Plus => IrUnaryOperatorKind.Identity,
            ExpressionOperatorKind.Minus => IrUnaryOperatorKind.Negate,
            ExpressionOperatorKind.BitNot => IrUnaryOperatorKind.Invert,
            ExpressionOperatorKind.Not => IrUnaryOperatorKind.Not,
            _ => throw new NotSupportedException($"IR: No support for Unary operator {syntax.OperatorKind}."),
        };
    }

    public IrUnaryOperatorKind Kind { get; }
    public TypeSymbol OperandType { get; }

    public new ExpressionOperatorSyntax Syntax
        => (ExpressionOperatorSyntax)base.Syntax;
}
