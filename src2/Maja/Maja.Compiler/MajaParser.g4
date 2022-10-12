parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilationUnit: (useDecl | pubDecl | newline)* (membersDecl | newline)*;

pubDecl: Pub freeSpace nameQualifiedList;
useDecl: Use Sp+ nameQualified;

codeBlock: (statement | membersDecl | newline)+;
membersDecl: functionDecl | typeDecl | variableDecl;

statement: statementFlow | statementExpression;
statementFlow: statementRet;
statementRet: Ret (Sp expression)?;
statementExpression: expression;

functionDecl: nameIdentifier Colon Sp typeParameterList? parameterList (Colon Sp type)? newline Indent codeBlock Dedent;
functionDeclLocal: Indent functionDecl Dedent;
parameterList: ParenOpen (parameter (Comma Sp parameter)*)? ParenClose;
parameter: nameIdentifier Colon Sp type;
argumentList: ParenOpen (argument (Comma Sp argument)*)? ParenClose;
argument: (nameIdentifier Eq)? expression;

typeDecl: nameIdentifier typeParameterList? (Colon Sp type)? (Discard newline | newline Indent typeDeclMembers Dedent);
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
expressionOperatorLogicUnaryPrefix: <assoc=right> Not;
expressionOperatorComparison: Eq | Neq | AngleClose | AngleOpen | GtEq | LtEq;
expressionOperatorBits: BitAnd | BitOr | BitXor_Imm | BitShiftL | Minus? AngleClose AngleClose | BitRollL | BitRollR;
expressionOperatorBitsUnaryPrefix: <assoc=right> BitNot;
expressionOperatorAssignment: <assoc=right> Eq;

expressionLiteralBool: True | False;
expressionLiteral: number | string;

nameQualified: nameIdentifier (Dot nameIdentifier)+;
nameQualifiedList: nameQualifiedListComma | nameQualifiedListIndent;
nameQualifiedListComma: nameQualified Sp? (Comma Sp+ nameQualified)*;
nameQualifiedListIndent: Indent (nameQualified newline)+ Dedent;

nameIdentifier: Identifier;
nameIdentifierList: nameQualifiedListComma | nameQualifiedListIndent;
nameIdentifierListComma: nameIdentifier Sp? (Comma Sp+ nameIdentifier)*;
nameIdentifierListIndent: Indent (nameIdentifier newline)+ Dedent;

string: String;
number: NumberBin 
    | NumberDec | NumberDecPrefix
    | NumberHex
    | NumberOct
    | Character;

newline: Sp* Comment? Eol;
freeSpace: Sp+ | Comment? Eol;