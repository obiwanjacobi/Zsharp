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
        switch (syntax.OperatorKind)
        {
            case ExpressionOperatorKind.And:
                return IrBinaryOperatorKind.And;
            case ExpressionOperatorKind.BitAnd:
                return IrBinaryOperatorKind.BitwiseAnd;
            case ExpressionOperatorKind.BitOr:
                return IrBinaryOperatorKind.BitwiseOr;
            case ExpressionOperatorKind.BitRollLeft:
                return IrBinaryOperatorKind.BitwiseRollLeft;
            case ExpressionOperatorKind.BitRollRight:
                return IrBinaryOperatorKind.BitwiseRollRight;
            case ExpressionOperatorKind.BitShiftLeft:
                return IrBinaryOperatorKind.BitwiseShiftLeft;
            case ExpressionOperatorKind.BitShiftRight:
                return IrBinaryOperatorKind.BitwiseShiftRight;
            case ExpressionOperatorKind.BitShiftRightSign:
                return IrBinaryOperatorKind.BitwiseShiftRightSign;
            case ExpressionOperatorKind.BitXor:
                return IrBinaryOperatorKind.BitwiseXor;
            case ExpressionOperatorKind.Divide:
                return IrBinaryOperatorKind.Divide;
            case ExpressionOperatorKind.Equals:
                return IrBinaryOperatorKind.Equals;
            case ExpressionOperatorKind.Greater:
                return IrBinaryOperatorKind.Greater;
            case ExpressionOperatorKind.GreaterOrEquals:
                return IrBinaryOperatorKind.GreaterOrEquals;
            case ExpressionOperatorKind.Lesser:
                return IrBinaryOperatorKind.Lesser;
            case ExpressionOperatorKind.LesserOrEquals:
                return IrBinaryOperatorKind.LesserOrEquals;
            case ExpressionOperatorKind.Minus:
                return IrBinaryOperatorKind.Subtract;
            case ExpressionOperatorKind.Modulo:
                return IrBinaryOperatorKind.Modulo;
            case ExpressionOperatorKind.Multiply:
                return IrBinaryOperatorKind.Multiply;
            case ExpressionOperatorKind.NotEquals:
                return IrBinaryOperatorKind.NotEquals;
            case ExpressionOperatorKind.Or:
                return IrBinaryOperatorKind.Or;
            case ExpressionOperatorKind.Plus:
                return IrBinaryOperatorKind.Add;
            case ExpressionOperatorKind.Power:
                return IrBinaryOperatorKind.Power;
            case ExpressionOperatorKind.Root:
                return IrBinaryOperatorKind.Root;
            case ExpressionOperatorKind.Xor:
                return IrBinaryOperatorKind.Xor;
        }

        throw new NotSupportedException($"IR: No support for Binary operator {syntax.OperatorKind}.");
    }

    public IrBinaryOperatorKind Kind { get; }
    public TypeSymbol OperandType { get; }
    public TypeSymbol TargetType { get; }
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
        switch (syntax.OperatorKind)
        {
            case ExpressionOperatorKind.Plus:
                return IrUnaryOperatorKind.Identity;
            case ExpressionOperatorKind.Minus:
                return IrUnaryOperatorKind.Negate;
            case ExpressionOperatorKind.BitNot:
                return IrUnaryOperatorKind.Invert;
            case ExpressionOperatorKind.Not:
                return IrUnaryOperatorKind.Not;
        }

        throw new NotSupportedException($"IR: No support for Unary operator {syntax.OperatorKind}.");
    }

    public IrUnaryOperatorKind Kind { get; }
    public TypeSymbol OperandType { get; }
}
