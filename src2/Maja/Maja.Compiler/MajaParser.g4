 // $antlr-format off
parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilationUnit: directiveMod? (directiveUse | directivePub | newline)* (declaration | statement | newline)* EOF;

directiveMod: Mod freeSpace nameQualified;
directivePub: Pub freeSpace nameQualifiedList;
directiveUse: Use freeSpace nameQualifiedList;

codeBlock: (statement | declaration | newline)+;
declaration: declarationPub | declarationVariable;
declarationPub: (Pub Sp)? (declarationFunction | declarationType);

statement: statementFlow | statementAssignment | statementExpression;
statementFlow: statementRet | statementIf | statementLoop;
statementIf: If Sp expression newline Indent codeBlock Dedent (statementElse | statementElseIf)?;
statementElse: Else newline Indent codeBlock Dedent;
statementElseIf: (Else freeSpace If | Elif) Sp expression newline Indent codeBlock Dedent (statementElse | statementElseIf)?;
statementRet: Ret (Sp expression)?;
statementAssignment: nameIdentifier Sp Eq Sp expression;
statementExpression: expression;
statementLoop: Loop (Sp expressionLoop)? newline Indent codeBlock Dedent;

declarationFunction: nameIdentifier Sp? Colon freeSpace typeParameterList? parameterList (Sp? Colon Sp type)? newline Indent codeBlock Dedent;
declarationFunctionLocal: Indent declarationFunction Dedent;
parameterList: ParenOpen (parameterListComma | newline parameterListIndent)? ParenClose;
parameterListComma: (parameter | compParameter) (Comma Sp (parameter | compParameter))*;
parameterListIndent: Indent (comment* (parameter | compParameter) newline)+ Dedent;
parameter: nameIdentifier Sp? Colon Sp type (Sp Eq Sp expression)?;
compParameter: Hash parameter;
argumentList: ParenOpen newline? (argumentListComma | argumentListIndent)? ParenClose;
argumentListComma: argument (Comma Sp argument)*;
argumentListIndent: Indent (argument newline)+ Dedent;
argument: (nameIdentifier Sp Eq Sp)? expression;

declarationType: nameIdentifier typeParameterList? (Sp? Colon Sp type)? newline Indent declarationTypeMemberList Dedent;
declarationTypeMemberList: (declarationTypeMemberListEnum | declarationTypeMemberListField | declarationTypeMemberListRule | newline)+;
declarationTypeMemberListEnum: (memberEnumValue newline)+ | ((memberEnum (Comma freeSpace memberEnum)*)+ newline);
declarationTypeMemberListField: (memberField newline)+;
declarationTypeMemberListRule: (memberRule newline)+;
type: nameIdentifier typeArgumentList? argumentList?;
typeParameterList: AngleOpen (typeParameterListComma | newline typeParameterListIndent) AngleClose;
typeParameterListComma: typeParameter (Comma Sp typeParameter)*;
typeParameterListIndent: Indent (comment* typeParameter newline)+ Dedent;
typeParameter: typeParameterGeneric | typeParameterTemplate;
typeParameterGeneric: type (Sp Eq Sp type)?;
typeParameterTemplate: Hash type (Sp Eq Sp type)?;
typeArgumentList: AngleOpen (typeArgumentListComma | newline typeArgumentListIndent) AngleClose;
typeArgumentListComma: typeArgument (Comma Sp typeArgument)*;
typeArgumentListIndent: Indent (typeArgument newline)+ Dedent;
typeArgument: type | expression;
typeInitializer: typeInitializerComma | newline typeInitializerIndent;
typeInitializerComma: CurlyOpen Sp typeInitializerField (Comma Sp typeInitializerField)* Sp CurlyClose;
typeInitializerIndent: Indent (typeInitializerField newline)+ Dedent;
typeInitializerField: nameIdentifier Sp Eq Sp expression;

memberEnumValue: nameIdentifier (Sp Eq Sp expressionConstant)?;
memberEnum: nameIdentifier;
memberField: nameIdentifier Sp? Colon Sp type (Sp Eq Sp expression)?;
memberRule: Hash expressionRule;

declarationVariable: declarationVariableTyped | declarationVariableInferred;
declarationVariableTyped: nameIdentifier Sp? Colon Sp type (Sp? Eq Sp expression)?;
declarationVariableInferred: nameIdentifier Sp? Colon Eq Sp expression;
variableAssignment: nameIdentifier Sp Eq Sp expression;

expression:
      expressionConstant                                        #expressionConst
    | expression Sp expressionOperatorBinary Sp expression      #expressionBinary
    | expressionOperatorUnaryPrefix expression                  #expressionUnaryPrefix
    | expression typeArgumentList? argumentList                 #expressionInvocation
    | type typeInitializer             				            #expressionTypeInitializer
    | BracketOpen expression? Range expression? BracketClose    #expressionRange
    | ParenOpen expression ParenClose                           #expressionPrecedence
    | nameIdentifier                                            #expressionIdentifier
    | expression Dot nameIdentifier                             #expressionMemberAccess
    ;
expressionConstant: expressionLiteral | expressionLiteralBool;
expressionRule: expression;
expressionLoop: expression;

expressionOperatorBinary: expressionOperatorArithmetic | expressionOperatorLogic | expressionOperatorComparison | expressionOperatorBits;
expressionOperatorUnaryPrefix: expressionOperatorArithmeticUnaryPrefix | expressionOperatorLogicUnaryPrefix | expressionOperatorBitsUnaryPrefix;

expressionOperatorArithmetic: Plus | Minus | Divide | Multiply | Modulo | Power | Root;
expressionOperatorArithmeticUnaryPrefix: Minus;
expressionOperatorLogic: And | Or;
expressionOperatorLogicUnaryPrefix: <assoc=right> Not;
expressionOperatorComparison: Eq | Neq | AngleClose | AngleOpen | GtEq | LtEq;
expressionOperatorBits: BitAnd | BitOr | BitXor_Imm | BitShiftL | AngleClose? AngleClose AngleClose | BitRollL | BitRollR;
expressionOperatorBitsUnaryPrefix: <assoc=right> BitNot;

expressionLiteralBool: True | False;
expressionLiteral: number | string;

nameQualified: nameIdentifier (Dot nameIdentifier)*;
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
newline: Sp* Comment? Eol+;
freeSpace: Sp+ | newline;
