using System;

namespace Zsharp.AST
{
    [Flags]
    public enum AstExpressionOperator
    {
        None = 0x0000,

        // to pass through a number
        Number = 0x0101,

        // precedence
        Open = 0x00C1,
        Close = 0x00C2,

        // arithmetic
        Plus = 0x1091,
        Minus = 0x1092,
        Divide = 0x10A3,
        Multiply = 0x10A4,
        Modulo = 0x10A5,
        Power = 0x10A6,
        Negate = 0x11B0,

        // comparison
        Equal = 0x2051,
        NotEqual = 0x2052,
        Greater = 0x2063,
        Smaller = 0x2064,
        GreaterEqual = 0x2065,
        SmallerEqual = 0x2066,

        // bitwise
        BitAnd = 0x4041,
        BitOr = 0x4022,
        BitXor = 0x4033,
        BitShiftLeft = 0x4074,
        BitShiftRight = 0x4075,
        BitRollLeft = 0x4076,
        BitRollRight = 0x4077,
        BitNegate = 0x41B0,

        // logic
        And = 0x8011,
        Or = 0x8002,
        Not = 0x81B0,

        // masks
        MaskArithmetic = 0x1000,
        MaskComparison = 0x2000,
        MaskBitwise = 0x4000,
        MaskLogic = 0x8000,
        MaskUnary = 0x0100,
        MaskPrecedence = 0x00F0,
    };

}
