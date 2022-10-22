namespace Maja.Compiler.Syntax;

public enum ExpressionOperatorKind
{
    Unknown,
    // assignment
    Assignment,
    // arithmetic
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    Power,
    Root,
    // logical
    And,
    Or,
    Xor,
    Not,
    //Nand,
    //Nor,
    //Nxor,
    // comparison
    Equals,
    NotEquals,
    Greater,
    GreaterOrEquals,
    Lesser,
    LesserOrEquals,
    // bitwise
    BitAnd,
    BitOr,
    BitXor,
    BitNot,
    BitShiftLeft,
    BitShiftRight,
    BitShiftRightSign,
    BitRollLeft,
    BitRollRight,
}

public enum ExpressionOperatorCategory
{
    Unknown,
    Arithmetic,
    Logic,
    Comparison,
    Bitwise,
    Assignment
}

public enum ExpressionOperatorCardinality
{
    Unknown,
    Unary, // prefix
    Binary,
    Ternary
}