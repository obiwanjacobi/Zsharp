//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:/My/MyProjects/Zsharp/src2/Maja/Maja.Compiler/MajaParser.g4 by ANTLR 4.13.1

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
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IMajaParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompilationUnit([NotNull] MajaParser.CompilationUnitContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.directiveMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectiveMod([NotNull] MajaParser.DirectiveModContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.directivePub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectivePub([NotNull] MajaParser.DirectivePubContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.directiveUse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectiveUse([NotNull] MajaParser.DirectiveUseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.codeBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCodeBlock([NotNull] MajaParser.CodeBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclaration([NotNull] MajaParser.DeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationPub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationPub([NotNull] MajaParser.DeclarationPubContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.statementIf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementIf([NotNull] MajaParser.StatementIfContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementElse([NotNull] MajaParser.StatementElseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementElseIf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementElseIf([NotNull] MajaParser.StatementElseIfContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementRet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementRet([NotNull] MajaParser.StatementRetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementAssignment([NotNull] MajaParser.StatementAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementExpression([NotNull] MajaParser.StatementExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.statementLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementLoop([NotNull] MajaParser.StatementLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationFunction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationFunction([NotNull] MajaParser.DeclarationFunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationFunctionLocal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationFunctionLocal([NotNull] MajaParser.DeclarationFunctionLocalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterList([NotNull] MajaParser.ParameterListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterListComma([NotNull] MajaParser.ParameterListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameterListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameterListIndent([NotNull] MajaParser.ParameterListIndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameter([NotNull] MajaParser.ParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.compParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompParameter([NotNull] MajaParser.CompParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argumentList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgumentList([NotNull] MajaParser.ArgumentListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argumentListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgumentListComma([NotNull] MajaParser.ArgumentListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argumentListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgumentListIndent([NotNull] MajaParser.ArgumentListIndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgument([NotNull] MajaParser.ArgumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationType([NotNull] MajaParser.DeclarationTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationTypeMemberList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationTypeMemberList([NotNull] MajaParser.DeclarationTypeMemberListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationTypeMemberListEnum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationTypeMemberListEnum([NotNull] MajaParser.DeclarationTypeMemberListEnumContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationTypeMemberListField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationTypeMemberListField([NotNull] MajaParser.DeclarationTypeMemberListFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationTypeMemberListRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationTypeMemberListRule([NotNull] MajaParser.DeclarationTypeMemberListRuleContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameterListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameterListComma([NotNull] MajaParser.TypeParameterListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameterListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameterListIndent([NotNull] MajaParser.TypeParameterListIndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameter([NotNull] MajaParser.TypeParameterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameterGeneric"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameterGeneric([NotNull] MajaParser.TypeParameterGenericContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeParameterTemplate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeParameterTemplate([NotNull] MajaParser.TypeParameterTemplateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgumentList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgumentList([NotNull] MajaParser.TypeArgumentListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgumentListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgumentListComma([NotNull] MajaParser.TypeArgumentListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgumentListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgumentListIndent([NotNull] MajaParser.TypeArgumentListIndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeArgument([NotNull] MajaParser.TypeArgumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeInitializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeInitializer([NotNull] MajaParser.TypeInitializerContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeInitializerComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeInitializerComma([NotNull] MajaParser.TypeInitializerCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeInitializerIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeInitializerIndent([NotNull] MajaParser.TypeInitializerIndentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.typeInitializerField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypeInitializerField([NotNull] MajaParser.TypeInitializerFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.memberEnumValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemberEnumValue([NotNull] MajaParser.MemberEnumValueContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.declarationVariable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationVariable([NotNull] MajaParser.DeclarationVariableContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationVariableTyped"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationVariableTyped([NotNull] MajaParser.DeclarationVariableTypedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.declarationVariableInferred"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclarationVariableInferred([NotNull] MajaParser.DeclarationVariableInferredContext context);
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
	/// Visit a parse tree produced by the <c>expressionMemberAccess</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionMemberAccess([NotNull] MajaParser.ExpressionMemberAccessContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionInvocation</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionInvocation([NotNull] MajaParser.ExpressionInvocationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionRange</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionRange([NotNull] MajaParser.ExpressionRangeContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionUnaryPrefix</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionUnaryPrefix([NotNull] MajaParser.ExpressionUnaryPrefixContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expressionTypeInitializer</c>
	/// labeled alternative in <see cref="MajaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionTypeInitializer([NotNull] MajaParser.ExpressionTypeInitializerContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.expressionLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionLoop([NotNull] MajaParser.ExpressionLoopContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.nameQualifiedListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameQualifiedListComma([NotNull] MajaParser.NameQualifiedListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameQualifiedListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameQualifiedListIndent([NotNull] MajaParser.NameQualifiedListIndentContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.nameIdentifierListComma"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameIdentifierListComma([NotNull] MajaParser.NameIdentifierListCommaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.nameIdentifierListIndent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNameIdentifierListIndent([NotNull] MajaParser.NameIdentifierListIndentContext context);
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
	/// Visit a parse tree produced by <see cref="MajaParser.comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComment([NotNull] MajaParser.CommentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.newline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNewline([NotNull] MajaParser.NewlineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MajaParser.freeSpace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFreeSpace([NotNull] MajaParser.FreeSpaceContext context);
}
} // namespace Maja.Compiler.Parser
