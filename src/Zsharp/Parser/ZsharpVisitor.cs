//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\Users\marc\Documents\MyProjects\Zsharp\src\Zsharp\Zsharp.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Zsharp.Parser {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="ZsharpParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface IZsharpVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.file"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFile([NotNull] ZsharpParser.FileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.source"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSource([NotNull] ZsharpParser.SourceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.codeblock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCodeblock([NotNull] ZsharpParser.CodeblockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.module_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitModule_statement([NotNull] ZsharpParser.Module_statementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.module_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitModule_name([NotNull] ZsharpParser.Module_nameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_module"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_module([NotNull] ZsharpParser.Statement_moduleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_import"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_import([NotNull] ZsharpParser.Statement_importContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_export"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_export([NotNull] ZsharpParser.Statement_exportContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.flow_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFlow_statement([NotNull] ZsharpParser.Flow_statementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_return"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_return([NotNull] ZsharpParser.Statement_returnContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_if([NotNull] ZsharpParser.Statement_ifContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_else"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_else([NotNull] ZsharpParser.Statement_elseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_elseif"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_elseif([NotNull] ZsharpParser.Statement_elseifContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_break"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_break([NotNull] ZsharpParser.Statement_breakContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_continue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_continue([NotNull] ZsharpParser.Statement_continueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_loop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_loop([NotNull] ZsharpParser.Statement_loopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_loop_infinite"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_loop_infinite([NotNull] ZsharpParser.Statement_loop_infiniteContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.statement_loop_while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_loop_while([NotNull] ZsharpParser.Statement_loop_whileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.definition_top"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefinition_top([NotNull] ZsharpParser.Definition_topContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefinition([NotNull] ZsharpParser.DefinitionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.expression_value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_value([NotNull] ZsharpParser.Expression_valueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.comptime_expression_value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComptime_expression_value([NotNull] ZsharpParser.Comptime_expression_valueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.expression_arithmetic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_arithmetic([NotNull] ZsharpParser.Expression_arithmeticContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.arithmetic_operand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArithmetic_operand([NotNull] ZsharpParser.Arithmetic_operandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.expression_logic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_logic([NotNull] ZsharpParser.Expression_logicContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.logic_operand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLogic_operand([NotNull] ZsharpParser.Logic_operandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.expression_comparison"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_comparison([NotNull] ZsharpParser.Expression_comparisonContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.comparison_operand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparison_operand([NotNull] ZsharpParser.Comparison_operandContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.expression_bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_bool([NotNull] ZsharpParser.Expression_boolContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_def([NotNull] ZsharpParser.Function_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_def_export"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_def_export([NotNull] ZsharpParser.Function_def_exportContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_parameter_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_parameter_list([NotNull] ZsharpParser.Function_parameter_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_parameter([NotNull] ZsharpParser.Function_parameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_parameter_self"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_parameter_self([NotNull] ZsharpParser.Function_parameter_selfContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_return_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_return_type([NotNull] ZsharpParser.Function_return_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_call"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_call([NotNull] ZsharpParser.Function_callContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_parameter_uselist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_parameter_uselist([NotNull] ZsharpParser.Function_parameter_uselistContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.function_param_use"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction_param_use([NotNull] ZsharpParser.Function_param_useContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_def_top"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_def_top([NotNull] ZsharpParser.Variable_def_topContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_def([NotNull] ZsharpParser.Variable_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_def_typed"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_def_typed([NotNull] ZsharpParser.Variable_def_typedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_def_typed_init"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_def_typed_init([NotNull] ZsharpParser.Variable_def_typed_initContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_assign_auto"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_assign_auto([NotNull] ZsharpParser.Variable_assign_autoContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_assign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_assign([NotNull] ZsharpParser.Variable_assignContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.variable_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_ref([NotNull] ZsharpParser.Variable_refContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.struct_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStruct_def([NotNull] ZsharpParser.Struct_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.struct_field_def_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStruct_field_def_list([NotNull] ZsharpParser.Struct_field_def_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.struct_field_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStruct_field_def([NotNull] ZsharpParser.Struct_field_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_def([NotNull] ZsharpParser.Enum_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_option_def_listline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_option_def_listline([NotNull] ZsharpParser.Enum_option_def_listlineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_option_def_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_option_def_list([NotNull] ZsharpParser.Enum_option_def_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_option_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_option_def([NotNull] ZsharpParser.Enum_option_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_option_value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_option_value([NotNull] ZsharpParser.Enum_option_valueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.enum_base_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum_base_type([NotNull] ZsharpParser.Enum_base_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_def([NotNull] ZsharpParser.Type_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_alias"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_alias([NotNull] ZsharpParser.Type_aliasContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_ref_use"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_ref_use([NotNull] ZsharpParser.Type_ref_useContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_ref([NotNull] ZsharpParser.Type_refContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_name([NotNull] ZsharpParser.Type_nameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.known_types"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitKnown_types([NotNull] ZsharpParser.Known_typesContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_Bit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_Bit([NotNull] ZsharpParser.Type_BitContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_Ptr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_Ptr([NotNull] ZsharpParser.Type_PtrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_Opt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_Opt([NotNull] ZsharpParser.Type_OptContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_Err"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_Err([NotNull] ZsharpParser.Type_ErrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.type_Imm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType_Imm([NotNull] ZsharpParser.Type_ImmContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.template_param_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTemplate_param_list([NotNull] ZsharpParser.Template_param_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.template_param_number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTemplate_param_number([NotNull] ZsharpParser.Template_param_numberContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.template_param_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTemplate_param_type([NotNull] ZsharpParser.Template_param_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.template_param_any"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTemplate_param_any([NotNull] ZsharpParser.Template_param_anyContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_type([NotNull] ZsharpParser.Identifier_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_var([NotNull] ZsharpParser.Identifier_varContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_param"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_param([NotNull] ZsharpParser.Identifier_paramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_func"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_func([NotNull] ZsharpParser.Identifier_funcContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_field"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_field([NotNull] ZsharpParser.Identifier_fieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_enumoption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_enumoption([NotNull] ZsharpParser.Identifier_enumoptionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_module"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_module([NotNull] ZsharpParser.Identifier_moduleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.identifier_unused"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier_unused([NotNull] ZsharpParser.Identifier_unusedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.literal_bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteral_bool([NotNull] ZsharpParser.Literal_boolContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteral([NotNull] ZsharpParser.LiteralContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber([NotNull] ZsharpParser.NumberContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number_bin"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber_bin([NotNull] ZsharpParser.Number_binContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number_oct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber_oct([NotNull] ZsharpParser.Number_octContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number_dec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber_dec([NotNull] ZsharpParser.Number_decContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number_hex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber_hex([NotNull] ZsharpParser.Number_hexContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.number_char"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber_char([NotNull] ZsharpParser.Number_charContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_arithmetic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_arithmetic([NotNull] ZsharpParser.Operator_arithmeticContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_arithmetic_unary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_arithmetic_unary([NotNull] ZsharpParser.Operator_arithmetic_unaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_logic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_logic([NotNull] ZsharpParser.Operator_logicContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_logic_unary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_logic_unary([NotNull] ZsharpParser.Operator_logic_unaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_comparison"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_comparison([NotNull] ZsharpParser.Operator_comparisonContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_bits"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_bits([NotNull] ZsharpParser.Operator_bitsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_bits_unary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_bits_unary([NotNull] ZsharpParser.Operator_bits_unaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.operator_assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator_assignment([NotNull] ZsharpParser.Operator_assignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.empty_line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEmpty_line([NotNull] ZsharpParser.Empty_lineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.newline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNewline([NotNull] ZsharpParser.NewlineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComment([NotNull] ZsharpParser.CommentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString([NotNull] ZsharpParser.StringContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.character"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCharacter([NotNull] ZsharpParser.CharacterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ZsharpParser.indent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIndent([NotNull] ZsharpParser.IndentContext context);
}
} // namespace Zsharp.Parser
