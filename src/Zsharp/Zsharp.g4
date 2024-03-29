grammar Zsharp;

// entry point
file : header* source* INDENT? EOF;
header: module_statement | comment | empty_line;
source: definition_top | function_use 
    | comment | empty_line;
codeblock: (flow_statement 
    | function_use | definition 
    | comment | empty_line)+;

// modules
module_statement : statement_module | statement_import | statement_export;
module_name: identifier_module | module_name DOT identifier_module;
module_namespace: module_name DOT MULT_PTR;
statement_module: MODULE SP module_name newline;
statement_import: IMPORT SP (((alias_module SP EQ_ASSIGN SP)? module_name) | module_namespace) newline;
statement_export: EXPORT SP (identifier_func | identifier_type) newline;
statement_export_inline: EXPORT SP (function_def | type_def | struct_def | enum_def);

// flow control
flow_statement: statement_if | statement_else | statement_elseif
    | statement_loop | statement_return | statement_break | statement_continue;
statement_return: indent RETURN (SP expression_value)? newline;
statement_if: indent IF SP expression_logic newline codeblock;
statement_else: indent ELSE newline codeblock;
statement_elseif: indent ELSE SP IF SP expression_logic newline codeblock;
statement_break: indent BREAK;
statement_continue: indent CONTINUE;
statement_loop: indent (statement_loop_infinite | statement_loop_iteration | statement_loop_while);
statement_loop_infinite: LOOP newline codeblock;
statement_loop_iteration: LOOP SP expression_iteration newline codeblock;
statement_loop_while: LOOP SP expression_logic newline codeblock;

// definition
definition_top: function_def | enum_def | struct_def 
    | type_def | type_alias | variable_def_top 
    | statement_export_inline;
definition: function_def | variable_def;

// expressions
expression_value: number | string 
    | function_use
    | variable_ref | variable_field_ref
    | enum_option_use | expression_bool
    | expression_arithmetic | expression_logic;
comptime_expression_value: number | string | expression_bool | enum_option_use;

expression_arithmetic: 
      expression_arithmetic SP operator_arithmetic SP expression_arithmetic
    | expression_arithmetic SP operator_bits SP expression_arithmetic
    | PARENopen expression_arithmetic PARENclose
    | operator_arithmetic_unary expression_arithmetic
    | operator_bits_unary expression_arithmetic
    | arithmetic_operand;
arithmetic_operand: number | variable_ref | function_use;

expression_logic: 
      expression_logic SP operator_logic SP expression_logic
    | PARENopen expression_logic PARENclose
    | operator_logic_unary SP expression_logic
    | logic_operand;
logic_operand: expression_bool | expression_comparison;

expression_comparison: 
      expression_comparison SP operator_comparison SP expression_comparison
    | PARENopen expression_comparison PARENclose
    | comparison_operand;
comparison_operand: function_use | variable_ref | literal | expression_arithmetic;

expression_bool: literal_bool | variable_ref | function_use;

expression_iteration: variable_ref SP IN SP MINUS_NEG? range;

// functions
function_def: identifier_func COLON SP template_param_list? PARENopen function_parameter_list? PARENclose function_return_type? newline codeblock;
function_parameter_list: (function_parameter | function_parameter_self) (COMMA SP function_parameter)*;
function_parameter: identifier_param type_ref_use;
function_parameter_self: SELF type_ref_use;
function_return_type: type_ref_use;
function_use: indent? (function_call | function_call_self | function_call_retval_unused);
function_call: identifier_func template_param_list_use? PARENopen function_parameter_uselist? PARENclose newline?;
function_parameter_uselist: function_param_use (COMMA SP function_param_use)*;
function_param_use: expression_value;
function_call_retval_unused: UNUSED SP EQ_ASSIGN SP (function_call | function_call_self);
function_call_self: variable_ref DOT function_call;

// variables
variable_def_top: variable_def_typed | variable_assign_value | variable_assign_struct;
variable_def: indent (variable_def_typed | variable_assign_value | variable_assign_struct);
variable_def_typed: identifier_var type_ref_use newline;
variable_assign_value: identifier_var type_ref_use? SP EQ_ASSIGN SP expression_value newline;
variable_assign_struct: identifier_var SP EQ_ASSIGN SP type_ref newline struct_field_init*;
variable_ref: SELF | identifier_var;
variable_field_ref: variable_ref DOT identifier_field;

// structs
struct_def: identifier_type template_param_list? (type_ref_use)? newline struct_field_def_list;
struct_field_def_list: struct_field_def+;
struct_field_def: indent identifier_field type_ref_use newline;
struct_field_init: indent identifier_field SP EQ_ASSIGN SP expression_value newline;

// enums
enum_def: identifier_type type_ref_use? newline (enum_option_def_list | enum_option_def_listline);
enum_option_def_listline: indent (identifier_enumoption COMMA SP)* identifier_enumoption COMMA? newline;
enum_option_def_list: (enum_option_def newline)* enum_option_def newline;
enum_option_def: indent identifier_enumoption enum_option_value?;
enum_option_value: SP EQ_ASSIGN SP comptime_expression_value;
enum_option_use: identifier_type DOT identifier_enumoption;

// types
type_def: identifier_type template_param_list? type_ref_use SP UNUSED newline;
type_alias: identifier_type template_param_list? SP EQ_ASSIGN SP type_ref newline;
type_ref_use: COLON SP type_ref;
type_ref: type_name ERROR? QUESTION?;

type_name: identifier_type template_param_list_use?;

// templates
template_param_list_use: SMALL_ANGLEopen template_param_use (COMMA SP template_param_use)* SP? GREAT_ANGLEclose;
template_param_use: type_ref | comptime_expression_value;
template_param_list: SMALL_ANGLEopen template_param_any (COMMA SP template_param_any)* GREAT_ANGLEclose;
template_param_var: identifier_param type_ref_use;
template_param_any: template_param_var | (COMPTIME? identifier_template_param);

// range
range: SUBopen range_begin? RANGE range_end? (COLON range_step)? SUBclose;
range_begin: expression_value;
range_end: expression_value;
range_step: expression_value;

// aliases
alias_module: identifier_module;

// identifiers
identifier_template_param: IDENTIFIERupper;
identifier_type: IDENTIFIERupper;
identifier_var: IDENTIFIERlower;
identifier_param: IDENTIFIERlower;
identifier_func: IDENTIFIERupper | IDENTIFIERlower;
identifier_field: IDENTIFIERupper | IDENTIFIERlower;
identifier_enumoption: IDENTIFIERupper | IDENTIFIERlower;
identifier_module: IDENTIFIERupper | IDENTIFIERlower;
identifier_unused: UNUSED;

literal_bool: TRUE | FALSE;
literal: number | string;

// numbers
number: NUMBERbin 
    | NUMBERoct 
    | NUMBERdec | NUMBERdec_prefix 
    | NUMBERhex
    | CHARACTER;

// operators
operator_arithmetic: PLUS | MINUS_NEG | DIV | MOD | MULT_PTR | POW;
operator_arithmetic_unary: MINUS_NEG;
operator_logic: AND | OR;
operator_logic_unary: NOT;
operator_comparison: EQ_ASSIGN | NEQ | GREAT_ANGLEclose | SMALL_ANGLEopen | GREQ | SMEQ;
operator_bits: BIT_AND | BIT_OR | BIT_XOR_IMM | BIT_SHL | MINUS_NEG? GREAT_ANGLEclose GREAT_ANGLEclose | BIT_ROLL | BIT_ROLR;
operator_bits_unary: BIT_NOT;
operator_assignment: EQ_ASSIGN;

empty_line: INDENT? EOL+;
newline: INDENT? COMMENT? EOL;
comment: INDENT? COMMENT EOL;
string: STRING;

indent: INDENT;

//
// Tokens
//

// keywords
MODULE: 'module';
IMPORT: 'import';
EXPORT: 'export';
LOOP: 'loop';
BREAK: 'break';
CONTINUE: 'continue';
IF: 'if';
ELSE: 'else';
RETURN: 'return';
IN: 'in';
SELF: 'self';
TRUE: 'true';
FALSE: 'false';

COMMENT: COMMENTstart .*? ~[\r\n]+;

NUMBERbin: PREFIXbin (DIGIT2 | UNUSED)+;
NUMBERoct: PREFIXoct (DIGIT8 | UNUSED)+;
NUMBERdec: DIGIT10+;
NUMBERdec_prefix: PREFIXdec (DIGIT10 | UNUSED)+;
NUMBERhex: PREFIXhex (DIGIT16 | UNUSED)+;

fragment ALPHAlower: [a-z];
fragment ALPHAupper: [A-Z];
fragment DIGIT2: [0-1];
fragment DIGIT8: [0-7];
fragment DIGIT10: [0-9];
fragment DIGIT16: [0-9a-fA-F];

// literals
fragment PREFIXbin: '0b';
fragment PREFIXoct: '0c';
fragment PREFIXdec: '0d';
fragment PREFIXhex: '0x';

CHARACTER: CHAR_QUOTE . CHAR_QUOTE;
STRING: STR_QUOTE .*? STR_QUOTE;

// operators
AND: 'and';
OR: 'or';
NOT: 'not';

UNUSED: '_';
PLUS: '+';
MINUS_NEG: '-';
MULT_PTR: '*';
DIV: '/';
MOD: '%';
POW: '**';
EQ_ASSIGN: '=';
NEQ: '<>';
GREAT_ANGLEclose: '>';
SMALL_ANGLEopen: '<';
GREQ: '>=';
SMEQ: '<=';
BIT_AND: '&';
BIT_OR: '|';
BIT_XOR_IMM: '^';
BIT_NOT: '~';
BIT_SHL: '<<';
//BIT_SHR: '>>';
BIT_ROLL: '|<';
BIT_ROLR: '>|';
CONCAT: '&&';
SUBopen: '[';
SUBclose: ']';
PARENopen: '(';
PARENclose: ')';
QUESTION: '?';
COLON: ':';
DOT: '.';
RANGE: '..';
SPREAD: '...';
COMMA: ',';
COMPTIME : '#';
COMPTIME_DEF: '#!';
ERROR: '!';
STR_QUOTE: '"';
CHAR_QUOTE: '\'';
COMMENTstart: '//';

// identifiers
IDENTIFIERupper: ALPHAupper IDENTIFIERpart*;
IDENTIFIERlower: ALPHAlower IDENTIFIERpart*;
fragment IDENTIFIERpart: ALPHAlower | ALPHAupper | DIGIT10 | UNUSED;

// whitespace
SP: ' ';
TAB: '\t';
INDENT: SP+;
EOL: '\r'? '\n' | '\r';
