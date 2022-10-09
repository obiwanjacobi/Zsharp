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
argumentList: ParenOpen (argument (Comma Sp argument)*)? ParenClose;
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

memberEnum: nameIdentifier (Sp Eq Sp expressionConstant)?;
memberField: nameIdentifier Colon Sp type;
memberRule: Hash nameIdentifier Sp expressionRule;

variableDecl: variableDeclTyped | variableDeclInferred;
variableDeclTyped: nameIdentifier Colon Sp type (Sp Eq Sp expression)?;
variableDeclInferred: nameIdentifier Sp Colon Eq Sp expression;
variableAssignment: nameIdentifier Sp Eq Sp expression;

expression: 
      expressionConstant                                    #expressionConst
    | expression Sp expressionOperatorBinary Sp expression  #expressionBinary
    | expressionOperatorUnaryPrefix expression              #expressionUnaryPrefix
    | ParenOpen expression ParenClose                       #expressionPrecedence
    | expression argumentList                               #expressionInvocation
    | nameIdentifier                                        #expressionIdentifier
    ;
expressionConstant: expressionLiteral | expressionLiteralBool;
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
expressionLiteral: number | string;

nameQualified: nameIdentifier (Dot nameIdentifier)+;
nameQualifiedList: nameQualified (Comma Sp+ nameQualified)*;
nameIdentifier: Identifier;
nameIdentifierList: nameIdentifier (Comma Sp+ nameIdentifier)*;

string: String;
number: NumberBin 
    | NumberDec | NumberDecPrefix
    | NumberHex
    | NumberOct
    | Character;

indent: Indent Sp+;
dedent: Dedent;
newline: Sp* Comment? Eol;
