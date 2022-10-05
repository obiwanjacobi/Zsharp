parser grammar MajaParser;
options { tokenVocab=MajaLexer; }

compilation_unit: (decl_use | decl_pub1 | decl_pub2 | newline)* (decl_member | newline)*;

decl_pub1: PUB SP+ name_qualified_list newline;
decl_pub2: PUB newline INDENT name_qualified_list newline DEDENT;
decl_use: USE SP+ name_qualified newline;

code_block: (statement | decl_member)+;
decl_member: decl_function | decl_type | decl_variable;

statement: statement_flow;
statement_flow: statement_ret;
statement_ret: RET;// SP+ expression?;

decl_function: name_identifier COLON SP type_parameter_list? parameter_list newline INDENT code_block DEDENT;
decl_function_local: INDENT decl_function DEDENT;
parameter_list: PARENopen (parameter | parameter_self (COMMA SP parameter)*)? PARENclose;
parameter: name_identifier COLON SP type;
parameter_self: SELF COLON SP type;

decl_type: name_identifier type_parameter_list? (COLON SP type)? UNUSED? newline (INDENT decl_type_members DEDENT)?;
decl_type_members: ((decl_member_enum | decl_member_field | decl_member_rule) newline)+;
type: name_identifier type_argument_list?;
type_parameter_list: ANGLEopen type_parameter (COMMA SP type_parameter)* ANGLEclose;
type_parameter: generic_parameter | template_parameter | value_parameter;
generic_parameter: name_identifier;
template_parameter: HASH name_identifier;
value_parameter: expression;
type_argument_list: ANGLEopen type_argument (COMMA SP type_argument)* ANGLEclose;
type_argument: name_identifier | expression;

decl_member_enum: name_identifier (SP EQ SP expression_const)?;
decl_member_field: name_identifier COLON SP type;
decl_member_rule: HASH name_rule SP expression_rule;
name_rule: name_identifier;

decl_variable: name_identifier SP? COLON (SP type)? (EQ expression)?;

expression: expression_const;
expression_const:;
expression_rule:;

name_qualified: name_identifier (DOT name_identifier)*;
name_qualified_list: name_qualified (COMMA SP+ name_qualified)*;
name_identifier: IDENTIFIER;
name_identifier_list: name_identifier (COMMA SP+ name_identifier)*;

newline: SP* COMMENT? EOL;
