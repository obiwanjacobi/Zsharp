//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\My\MyProjects\Zsharp\src2\Maja\Maja.Compiler\MajaParser.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Maja.Compiler.Parser {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="MajaParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IMajaParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompilationUnit([NotNull] MajaParser.CompilationUnitContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.pub1Decl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPub1Decl([NotNull] MajaParser.Pub1DeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.pub2Decl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPub2Decl([NotNull] MajaParser.Pub2DeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.useDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUseDecl([NotNull] MajaParser.UseDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.codeBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCodeBlock([NotNull] MajaParser.CodeBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.membersDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMembersDecl([NotNull] MajaParser.MembersDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] MajaParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementFlow"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementFlow([NotNull] MajaParser.StatementFlowContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementRet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementRet([NotNull] MajaParser.StatementRetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementExpression([NotNull] MajaParser.StatementExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.functionDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunctionDecl([NotNull] MajaParser.FunctionDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.functionDeclLocal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunctionDeclLocal([NotNull] MajaParser.FunctionDeclLocalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterList([NotNull] MajaParser.ParameterListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameter([NotNull] MajaParser.ParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argumentList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgumentList([NotNull] MajaParser.ArgumentListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgument([NotNull] MajaParser.ArgumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeDecl([NotNull] MajaParser.TypeDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeDeclMembers"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeDeclMembers([NotNull] MajaParser.TypeDeclMembersContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType([NotNull] MajaParser.TypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameterList([NotNull] MajaParser.TypeParameterListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameter([NotNull] MajaParser.TypeParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterGeneric"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterGeneric([NotNull] MajaParser.ParameterGenericContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterTemplate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterTemplate([NotNull] MajaParser.ParameterTemplateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterValue([NotNull] MajaParser.ParameterValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgumentList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgumentList([NotNull] MajaParser.TypeArgumentListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgument([NotNull] MajaParser.TypeArgumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.memberEnum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemberEnum([NotNull] MajaParser.MemberEnumContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.memberField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemberField([NotNull] MajaParser.MemberFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.memberRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemberRule([NotNull] MajaParser.MemberRuleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.variableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableDecl([NotNull] MajaParser.VariableDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.variableDeclTyped"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableDeclTyped([NotNull] MajaParser.VariableDeclTypedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.variableDeclInferred"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableDeclInferred([NotNull] MajaParser.VariableDeclInferredContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.variableAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableAssignment([NotNull] MajaParser.VariableAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionIdentifier</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionIdentifier([NotNull] MajaParser.ExpressionIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionInvocation</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionInvocation([NotNull] MajaParser.ExpressionInvocationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionUnaryPrefix</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionUnaryPrefix([NotNull] MajaParser.ExpressionUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionBinary</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionBinary([NotNull] MajaParser.ExpressionBinaryContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionConst</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionConst([NotNull] MajaParser.ExpressionConstContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionPrecedence</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionPrecedence([NotNull] MajaParser.ExpressionPrecedenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionConstant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionConstant([NotNull] MajaParser.ExpressionConstantContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionRule([NotNull] MajaParser.ExpressionRuleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorBinary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorBinary([NotNull] MajaParser.ExpressionOperatorBinaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorUnaryPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorUnaryPrefix([NotNull] MajaParser.ExpressionOperatorUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorArithmetic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorArithmetic([NotNull] MajaParser.ExpressionOperatorArithmeticContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorArithmeticUnaryPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorArithmeticUnaryPrefix([NotNull] MajaParser.ExpressionOperatorArithmeticUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorLogic"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorLogic([NotNull] MajaParser.ExpressionOperatorLogicContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorLogicUnaryPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorLogicUnaryPrefix([NotNull] MajaParser.ExpressionOperatorLogicUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorComparison"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorComparison([NotNull] MajaParser.ExpressionOperatorComparisonContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorBits"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorBits([NotNull] MajaParser.ExpressionOperatorBitsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorBitsUnaryPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorBitsUnaryPrefix([NotNull] MajaParser.ExpressionOperatorBitsUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionOperatorAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionOperatorAssignment([NotNull] MajaParser.ExpressionOperatorAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionLiteralBool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionLiteralBool([NotNull] MajaParser.ExpressionLiteralBoolContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.expressionLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionLiteral([NotNull] MajaParser.ExpressionLiteralContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameQualified"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameQualified([NotNull] MajaParser.NameQualifiedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameQualifiedList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameQualifiedList([NotNull] MajaParser.NameQualifiedListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameIdentifier([NotNull] MajaParser.NameIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameIdentifierList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameIdentifierList([NotNull] MajaParser.NameIdentifierListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString([NotNull] MajaParser.StringContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber([NotNull] MajaParser.NumberContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.indent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIndent([NotNull] MajaParser.IndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.dedent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDedent([NotNull] MajaParser.DedentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.newline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNewline([NotNull] MajaParser.NewlineContext context);
}
} // namespace Maja.Compiler.Parser
