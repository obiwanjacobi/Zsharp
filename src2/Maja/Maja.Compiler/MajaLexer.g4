lexer grammar MajaLexer;

tokens { Indent, Dedent }
options { superClass=Dentlr.DentlrLexer; }

CommentStart: '#_';
CommentWarning: '##';
Comment: (CommentStart | CommentWarning) .*? ~[\r\n]+;

Mod: 'mod';
Pub: 'pub';
Use: 'use';
Self: 'self';
Ret: 'ret';
Brk: 'brk';
Cnt: 'cnt';
Loop: 'loop';
If: 'if';
Else: 'else';
Elif: 'elif';
True: 'true';
False: 'false';
In: 'in';
Not: 'not';
And: 'and';
Or: 'or';

NumberBin: PREFIXbin (DIGIT2 | Discard)+;
NumberOct: PREFIXoct (DIGIT8 | Discard)+;
NumberDec: DIGIT10+;
NumberDecPrefix: PREFIXdec (DIGIT10 | Discard)+;
NumberHex: PREFIXhex (DIGIT16 | Discard)+;

Identifier: ALPHA (ALPHA | DIGIT10 | Discard)*;
fragment ALPHA: [a-zA-Z];
fragment DIGIT2: [0-1];
fragment DIGIT8: [0-7];
fragment DIGIT10: [0-9];
fragment DIGIT16: [0-9a-fA-F];

fragment PREFIXbin: '0b';
fragment PREFIXoct: '0c';
fragment PREFIXdec: '0d';
fragment PREFIXhex: '0x';

Character: CharQuote . CharQuote;
String: StrQuote .*? StrQuote;

ParenOpen: '(';
ParenClose: ')';
AngleOpen: '<';
AngleClose: '>';
BracketOpen: '[';
BracketClose: ']';
Hash: '#';
Colon: ':';
Dot: '.';
Range: '..';
Spread: '...';
Eq: '=';
Neq: '<>';
// Gt: '>';
// Lt: '<';
GtEq: '>=';
LtEq: '=<';
Plus: '+';
Minus: '-';
Multiply: '*';
Power: '**';
Divide: '/';
Root: '//';
Modulo: '%';
BitAnd: '&';
BitOr: '|';
BitNot: '~';
BitXor_Imm: '^';
BitShiftL: '<<';
//BitShiftR: '>>';  // parsed as 2 separate '>'
BitRollL: '|<';
BitRollR: '>|';
Question: '?';
Dollar: '$';
At: '@';
Error: '!';
StrQuote: '"';
CharQuote: '\'';
BackTick: '`';
Discard: '_';
Comma: ',';
Sp: ' ';
Eol: ('\r'? '\n' | '\r') | EOF;
