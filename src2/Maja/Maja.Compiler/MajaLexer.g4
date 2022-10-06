lexer grammar MajaLexer;

tokens { INDENT, DEDENT }
options { superClass=Dentlr.DentlrLexer; }

COMMENTstart: '#_';
COMMENTwarning: '##';
COMMENT: (COMMENTstart | COMMENTwarning) .*? ~[\r\n]+;

RET: 'ret';
PUB: 'pub';
USE: 'use';
SELF: 'self';

IDENTIFIER: ALPHA (ALPHA | DIGIT10 | DISCARD)*;
fragment ALPHA: [a-zA-Z];
fragment DIGIT2: [0-1];
fragment DIGIT8: [0-7];
fragment DIGIT10: [0-9];
fragment DIGIT16: [0-9a-fA-F];

PARENopen: '(';
PARENclose: ')';
ANGLEopen: '<';
ANGLEclose: '>';
HASH: '#';
COLON: ':';
DOT: '.';
EQ: '=';
DISCARD: '_';
COMMA: ',';
SP: ' ';
EOL: ('\r'? '\n' | '\r') | EOF;
