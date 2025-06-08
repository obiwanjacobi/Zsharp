using System;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal enum IrOperatorBinaryKind
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

internal sealed class IrOperatorBinary : IrNode
{
    public IrOperatorBinary(IrOperatorBinaryKind operatorKind, TypeSymbol operandType)
    {
        Kind = operatorKind;
        OperandType = operandType;
        TargetType = DetermineTargetType(operandType);
    }
    public IrOperatorBinary(ExpressionOperatorSyntax syntax, TypeSymbol operandType)
        : base(syntax)
    {
        Kind = DetermineKind(syntax);
        OperandType = operandType;
        TargetType = DetermineTargetType(operandType);
    }

    private TypeSymbol DetermineTargetType(TypeSymbol operandType)
    {
        switch (Kind)
        {
            case IrOperatorBinaryKind.Equals:
            case IrOperatorBinaryKind.NotEquals:
            case IrOperatorBinaryKind.Greater:
            case IrOperatorBinaryKind.GreaterOrEquals:
            case IrOperatorBinaryKind.Lesser:
            case IrOperatorBinaryKind.LesserOrEquals:
                return TypeSymbol.Bool;
            default:
                return operandType;
        }
    }

    private static IrOperatorBinaryKind DetermineKind(ExpressionOperatorSyntax syntax)
    {
        return syntax.OperatorKind switch
        {
            ExpressionOperatorKind.And => IrOperatorBinaryKind.And,
            ExpressionOperatorKind.BitAnd => IrOperatorBinaryKind.BitwiseAnd,
            ExpressionOperatorKind.BitOr => IrOperatorBinaryKind.BitwiseOr,
            ExpressionOperatorKind.BitRollLeft => IrOperatorBinaryKind.BitwiseRollLeft,
            ExpressionOperatorKind.BitRollRight => IrOperatorBinaryKind.BitwiseRollRight,
            ExpressionOperatorKind.BitShiftLeft => IrOperatorBinaryKind.BitwiseShiftLeft,
            ExpressionOperatorKind.BitShiftRight => IrOperatorBinaryKind.BitwiseShiftRight,
            ExpressionOperatorKind.BitShiftRightSign => IrOperatorBinaryKind.BitwiseShiftRightSign,
            ExpressionOperatorKind.BitXor => IrOperatorBinaryKind.BitwiseXor,
            ExpressionOperatorKind.Divide => IrOperatorBinaryKind.Divide,
            ExpressionOperatorKind.Equals => IrOperatorBinaryKind.Equals,
            ExpressionOperatorKind.Greater => IrOperatorBinaryKind.Greater,
            ExpressionOperatorKind.GreaterOrEquals => IrOperatorBinaryKind.GreaterOrEquals,
            ExpressionOperatorKind.Lesser => IrOperatorBinaryKind.Lesser,
            ExpressionOperatorKind.LesserOrEquals => IrOperatorBinaryKind.LesserOrEquals,
            ExpressionOperatorKind.Minus => IrOperatorBinaryKind.Subtract,
            ExpressionOperatorKind.Modulo => IrOperatorBinaryKind.Modulo,
            ExpressionOperatorKind.Multiply => IrOperatorBinaryKind.Multiply,
            ExpressionOperatorKind.NotEquals => IrOperatorBinaryKind.NotEquals,
            ExpressionOperatorKind.Or => IrOperatorBinaryKind.Or,
            ExpressionOperatorKind.Plus => IrOperatorBinaryKind.Add,
            ExpressionOperatorKind.Power => IrOperatorBinaryKind.Power,
            ExpressionOperatorKind.Root => IrOperatorBinaryKind.Root,
            ExpressionOperatorKind.Xor => IrOperatorBinaryKind.Xor,
            _ => throw new NotSupportedException($"IR: No support for Binary operator {syntax.OperatorKind}."),
        };
    }

    public IrOperatorBinaryKind Kind { get; }
    public TypeSymbol OperandType { get; }
    public TypeSymbol TargetType { get; }

    public new ExpressionOperatorSyntax Syntax
        => (ExpressionOperatorSyntax)base.Syntax;
}

internal enum IrOperatorUnaryKind
{
    Identity,   // plus does nothing
    Negate,     // arithmetic
    Invert,     // bitwise
    Not,        // logic
}

internal sealed class IrOperatorUnary : IrNode
{
    public IrOperatorUnary(ExpressionOperatorSyntax syntax, TypeSymbol operandType)
        : base(syntax)
    {
        Kind = DetermineKind(syntax);
        OperandType = operandType;
    }

    private static IrOperatorUnaryKind DetermineKind(ExpressionOperatorSyntax syntax)
    {
        return syntax.OperatorKind switch
        {
            ExpressionOperatorKind.Plus => IrOperatorUnaryKind.Identity,
            ExpressionOperatorKind.Minus => IrOperatorUnaryKind.Negate,
            ExpressionOperatorKind.BitNot => IrOperatorUnaryKind.Invert,
            ExpressionOperatorKind.Not => IrOperatorUnaryKind.Not,
            _ => throw new NotSupportedException($"IR: No support for Unary operator {syntax.OperatorKind}."),
        };
    }

    public IrOperatorUnaryKind Kind { get; }
    public TypeSymbol OperandType { get; }

    public new ExpressionOperatorSyntax Syntax
        => (ExpressionOperatorSyntax)base.Syntax;
}

internal sealed class IrOperatorAssignment : IrNode
{
    // for code generation
    private IrOperatorAssignment()
        : base(null)
    { }
    public IrOperatorAssignment(OperatorAssignmentSyntax syntax, TypeSymbol? wrapperType)
        : base(syntax)
    {
        WrapperType = wrapperType;
    }

    public TypeSymbol? WrapperType { get; }
    public bool CopyInstance
        => Syntax.Operators.Any(t => OperatorAssignmentSyntax.DetermineKind(t) == AssignmentOperatorKind.Copy);

    public new OperatorAssignmentSyntax Syntax
        => (OperatorAssignmentSyntax)base.Syntax;

    /// <summary>
    /// Plain and simple assignment: '='
    /// </summary>
    internal static IrOperatorAssignment Assign()
        => new();
}
