parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilationUnit: (useDecl | pubDecl | newline)* (membersDecl | statement | newline)*;

pubDecl: Pub freeSpace nameQualifiedList;
useDecl: Use Sp+ nameQualified;

codeBlock: (statement | membersDecl | newline)+;
membersDecl: functionDecl | typeDecl | variableDecl;

statement: statementFlow | statementExpression;
statementFlow: statementRet | statementIf;
statementIf: If Sp expression newline Indent codeBlock Dedent (statementElse | statementElseIf)?;
statementElse: Else newline Indent codeBlock Dedent;
statementElseIf: (Else freeSpace If | Elif) Sp expression newline Indent codeBlock Dedent (statementElse | statementElseIf)?;
statementRet: Ret (Sp expression)?;
statementExpression: expression;

functionDecl: nameIdentifier Colon freeSpace typeParameterList? parameterList (Colon Sp type)? newline Indent codeBlock Dedent;
functionDeclLocal: Indent functionDecl Dedent;
parameterList: ParenOpen (parameterListComma | newline parameterListIndent)? ParenClose;
parameterListComma: parameter (Comma Sp parameter)*;
parameterListIndent: Indent (comment* parameter newline)+ Dedent;
parameter: nameIdentifier Colon Sp type (Eq expression)?;
argumentList: ParenOpen newline? (argumentListComma | argumentListIndent)? ParenClose;
argumentListComma: argument (Comma Sp argument)*;
argumentListIndent: Indent (argument newline)+ Dedent;
argument: (nameIdentifier Eq)? expression;

typeDecl: nameIdentifier typeParameterList? (Colon Sp type)? newline Indent typeDeclMemberList Dedent;
typeDeclMemberList: (typeDeclMemberListEnum | typeDeclMemberListField | typeDeclMemberListRule)+;
typeDeclMemberListEnum: (memberEnumValue newline)+ | ((memberEnum (Comma freeSpace memberEnum)*)+ newline);
typeDeclMemberListField: (memberField newline)+;
typeDeclMemberListRule: (memberRule newline)+;
type: nameIdentifier typeArgumentList?;
typeParameterList: AngleOpen (typeParameterListComma | newline typeParameterListIndent) AngleClose;
typeParameterListComma: typeParameter (Comma Sp typeParameter)*;
typeParameterListIndent: Indent (comment* typeParameter newline)+ Dedent;
typeParameter: typeParameterGeneric | typeParameterTemplate | typeParameterValue;
typeParameterGeneric: nameIdentifier (Colon Sp? type)?;
typeParameterTemplate: Hash nameIdentifier (Colon Sp? type)?;
typeParameterValue: nameIdentifier Colon Sp type (Eq expression)?;
typeArgumentList: AngleOpen (typeArgumentListComma | typeArgumentListIndent) AngleClose;
typeArgumentListComma: typeArgument (Comma Sp typeArgument)*;
typeArgumentListIndent: Indent (typeArgument newline)+ Dedent;
typeArgument: type | expression;

memberEnumValue: nameIdentifier (Sp Eq Sp expressionConstant)?;
memberEnum: nameIdentifier;
memberField: nameIdentifier Colon Sp type (Sp Eq Sp expression)?;
memberRule: Hash nameIdentifier Sp expressionRule;

variableDecl: variableDeclTyped | variableDeclInferred;
variableDeclTyped: nameIdentifier Colon Sp type (Sp Eq Sp expression)?;
variableDeclInferred: nameIdentifier Sp Colon Eq Sp expression;
variableAssignment: nameIdentifier Sp Eq Sp expression;

expression: 
      expressionConstant                                    #expressionConst
    | expression Sp expressionOperatorBinary Sp expression  #expressionBinary
    | expressionOperatorUnaryPrefix expression              #expressionUnaryPrefix
    | expression argumentList                               #expressionInvocation
    | ParenOpen expression ParenClose                       #expressionPrecedence
    | nameIdentifier                                        #expressionIdentifier
    ;
expressionConstant: expressionLiteral | expressionLiteralBool;
expressionRule: Hash Identifier expression;

expressionOperatorBinary: expressionOperatorArithmetic | expressionOperatorLogic | expressionOperatorComparison | expressionOperatorBits;
expressionOperatorUnaryPrefix: expressionOperatorArithmeticUnaryPrefix | expressionOperatorLogicUnaryPrefix | expressionOperatorBitsUnaryPrefix;

expressionOperatorArithmetic: Plus | Minus | Divide | Multiply | Modulo | Power | Root;
expressionOperatorArithmeticUnaryPrefix: Minus;
expressionOperatorLogic: And | Or;
expressionOperatorLogicUnaryPrefix: <assoc=right> Not;
expressionOperatorComparison: Eq | Neq | AngleClose | AngleOpen | GtEq | LtEq;
expressionOperatorBits: BitAnd | BitOr | BitXor_Imm | BitShiftL | AngleClose? AngleClose AngleClose | BitRollL | BitRollR;
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

comment: Sp* Comment Eol;
newline: Sp* Comment? Eol;
freeSpace: Sp+ | newline;