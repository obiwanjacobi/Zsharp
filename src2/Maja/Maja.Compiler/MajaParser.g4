parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilationUnit: (useDecl | pub1Decl | pub2Decl | newline)* (membersDecl | newline)*;

pub1Decl: Pub Sp+ nameQualifiedList newline;
pub2Decl: Pub newline indent nameQualifiedList newline dedent;
useDecl: Use Sp+ nameQualified newline;

codeBlock: (statement | membersDecl | newline)+;
membersDecl: functionDecl | typeDecl | variableDecl;

statement: statementFlow | statementExpression;
statementFlow: statementRet;
statementRet: Ret (Sp expression)?;
statementExpression: expression;

functionDecl: nameIdentifier Colon Sp typeParameterList? parameterList (Colon Sp type)? newline indent codeBlock dedent;
functionDeclLocal: indent functionDecl dedent;
parameterList: ParenOpen (parameter (Comma Sp parameter)*)? ParenClose;
parameter: nameIdentifier Colon Sp type;
argumentList: ParenOpen (argument (Comma argument)*)? ParenClose;
argument: (nameIdentifier Eq)? expression;

typeDecl: nameIdentifier typeParameterList? (Colon Sp type)? (Discard newline | newline indent typeDeclMembers dedent);
typeDeclMembers: ((memberEnum | memberField | memberRule) newline)+;
type: nameIdentifier typeArgumentList?;
typeParameterList: AngleOpen typeParameter (Comma Sp typeParameter)* AngleClose;
typeParameter: parameterGeneric | parameterTemplate | parameterValue;
parameterGeneric: nameIdentifier;
parameterTemplate: Hash nameIdentifier;
parameterValue: expression;
typeArgumentList: AngleOpen typeArgument (Comma Sp typeArgument)* AngleClose;
typeArgument: nameIdentifier | expression;

memberEnum: nameIdentifier (Sp Eq Sp expressionConst)?;
memberField: nameIdentifier Colon Sp type;
memberRule: Hash nameIdentifier Sp expressionRule;

variableDecl: nameIdentifier Sp? Colon (Sp type)? (Eq Sp expression)?;
variableAssignment: nameIdentifier Sp Eq Sp expression;

expression: expressionConst
    | expression expressionOperatorBinary expression    // binary expression
    | expressionOperatorUnaryPrefix expression          // unary expression
    | ParenOpen expression ParenClose                   // precendence
    | expression argumentList                           // invocation expression
    ;
expressionConst: expressionLiteral | expressionLiteralBool;
expressionRule:;

expressionOperatorBinary: expressionOperatorArithmetic | expressionOperatorLogic | expressionOperatorComparison | expressionOperatorBits;
expressionOperatorUnaryPrefix: expressionOperatorArithmeticUnaryPrefix | expressionOperatorLogicUnaryPrefix | expressionOperatorBitsUnaryPrefix;

expressionOperatorArithmetic: Plus | Minus | Divide | Multiply | Mod | Power | Root;
expressionOperatorArithmeticUnaryPrefix: Minus;
expressionOperatorLogic: And | Or;
expressionOperatorLogicUnaryPrefix: Not;
expressionOperatorComparison: Eq | Neq | AngleClose | AngleOpen | GtEq | LtEq;
expressionOperatorBits: BitAnd | BitOr | BitXor_Imm | BitShiftL | Minus? AngleClose AngleClose | BitRollL | BitRollR;
expressionOperatorBitsUnaryPrefix: BitNot;
expressionOperatorAssignment: Eq;

expressionLiteralBool: True | False;
expressionLiteral: number | String;

nameQualified: nameIdentifier (Dot nameIdentifier)+;
nameQualifiedList: nameQualified (Comma Sp+ nameQualified)*;
nameIdentifier: Identifier;
nameIdentifierList: nameIdentifier (Comma Sp+ nameIdentifier)*;

number: NumberBin 
    | NumberDec | NumberDecPrefix
    | NumberHex
    | NumberOct
    | Character;

indent: Indent Sp+;
dedent: Dedent;
newline: Sp* Comment? Eol;
