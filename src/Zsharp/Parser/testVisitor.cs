//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\Users\marc\Documents\MyProjects\Zalt\Zsharp\Zsharp\test.g4_ by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="testParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface ItestVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.run"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRun([NotNull] testParser.RunContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.code"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCode([NotNull] testParser.CodeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.fn_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFn_def([NotNull] testParser.Fn_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.fn_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFn_type([NotNull] testParser.Fn_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.fn_param_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFn_param_list([NotNull] testParser.Fn_param_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.fn_param"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFn_param([NotNull] testParser.Fn_paramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.var_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVar_def([NotNull] testParser.Var_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType([NotNull] testParser.TypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="testParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier([NotNull] testParser.IdentifierContext context);
}
