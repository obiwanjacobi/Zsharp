module ZSharp.Ast

open FParsec

// parser types

type ParserState = {
    IndentCharCount: int
} with
    static member Default = {
        IndentCharCount = 0
    }

type VoidParser = Parser<unit, ParserState>
type Parser<'r> = Parser<'r, ParserState>

// general types

type Name = string

// Ast

type AstIndent = int
type AstIdentifier = string
type AstComment = { Location: Position; Comment: string }
type AstModule = { Location: Position; ModuleName: Name }

type AstExpressionOperator =
    | ArithmeticAdd | ArithmeticSubtract | ArithmeticMultiply | ArithmeticDivide | ArithmeticPower | ArithmeticRoot | ArithmeticNegate
    | CompareEquals | CompareGreater | CompareGreaterEquals | CompareSmaller | CompareSmallerEquals
    | LogicAnd | LogicOr | LogicNot
    | BitwiseAnd | BitwiseOr | BitwiseNot | BitwiseShiftLeft | BitwiseShiftRight

type AstIntLiteral = { Location: Position; Value: int64 }
type AstFloatLiteral = { Location: Position; Value: double }
type AstBoolLiteral = { Location: Position; Value: bool }
type AstStringLiteral = { Location: Position; Value: string }
type AstIdentifierRef = { Location: Position; Identifier: AstIdentifier }

type AstExpression = 
    | IntLiteral of AstIntLiteral
    | FloatLiteral of AstFloatLiteral
    | BoolLiteral of AstBoolLiteral
    | StringLiteral of AstStringLiteral
    | Identifier of AstIdentifierRef
    | Infix of AstExpression * AstExpression * AstExpressionOperator
    | Prefix of AstExpression * AstExpressionOperator

type AstVariableList = {
    Location: Position;
    Variables: AstIdentifier list
}

type AstAssignment = {
    Location: Position;
    Indent: AstIndent;
    Variables: AstVariableList;
    Expression: AstExpression
}

type AstLoopConditional = {
    Indent: AstIndent;
    Location: Position;
    Expression: AstExpression option;
}

type AstLoopRange = {
    Indent: AstIndent;
    Location: Position;
    Variables: AstVariableList;
    Expression: AstExpression;
}

type AstLoop =
    | ConditionalLoop of AstLoopConditional
    | RangeLoop of AstLoopRange

type AstStatement =
    | Assignment of AstAssignment
    | Loop of AstLoop

type AstCodeBlock = {
    Comments: AstComment list;
    Statements: AstStatement list;
}

type AstFile = {
    Name: Name
    Module: AstModule option
    CodeBlock: AstCodeBlock
}
