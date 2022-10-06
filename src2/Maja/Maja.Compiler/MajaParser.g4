parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilation_unit: (use_decl | pub1_decl | pub2_decl | newline)* (members_decl | newline)*;

pub1_decl: PUB SP+ name_qualified_list newline;
pub2_decl: PUB newline indent name_qualified_list newline dedent;
use_decl: USE SP+ name_qualified newline;

code_block: (statement | members_decl | newline)+;
members_decl: function_decl | type_decl | variable_decl;

statement: statement_flow;
statement_flow: statement_ret;
statement_ret: RET;// SP expression?;

function_decl: name_identifier COLON SP type_parameter_list? parameter_list newline indent code_block dedent;
function_decl_local: indent function_decl dedent;
parameter_list: PARENopen (parameter | parameter_self (COMMA SP parameter)*)? PARENclose;
parameter: name_identifier COLON SP type;
parameter_self: SELF COLON SP type;

type_decl: name_identifier type_parameter_list? (COLON SP type)? DISCARD? newline (indent type_decl_members dedent)?;
type_decl_members: ((member_enum | member_field | member_rule) newline)+;
type: name_identifier type_argument_list?;
type_parameter_list: ANGLEopen type_parameter (COMMA SP type_parameter)* ANGLEclose;
type_parameter: generic_parameter | template_parameter | value_parameter;
generic_parameter: name_identifier;
template_parameter: HASH name_identifier;
value_parameter: expression;
type_argument_list: ANGLEopen type_argument (COMMA SP type_argument)* ANGLEclose;
type_argument: name_identifier | expression;

member_enum: name_identifier (SP EQ SP expression_const)?;
member_field: name_identifier COLON SP type;
member_rule: HASH name_identifier SP expression_rule;

variable_decl: name_identifier SP? COLON (SP type)? (EQ expression)?;

expression: expression_const;
expression_const:;
expression_rule:;

name_qualified: name_identifier (DOT name_identifier)*;
name_qualified_list: name_qualified (COMMA SP+ name_qualified)*;
name_identifier: IDENTIFIER;
name_identifier_list: name_identifier (COMMA SP+ name_identifier)*;

indent: INDENT SP+;
dedent: DEDENT;
newline: SP* COMMENT? EOL;
