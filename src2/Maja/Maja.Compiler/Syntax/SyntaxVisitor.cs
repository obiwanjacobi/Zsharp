namespace Maja.Compiler.Syntax;

/// <summary>
/// The syntax node visitor interface.
/// </summary>
/// <typeparam name="R">The return type of all the VisitXxxx methods.</typeparam>
public interface ISyntaxVisitor<R>
{
    R Default { get; }
    R AggregateResult(R aggregate, R newResult);

    R OnCompilationUnit(CompilationUnitSyntax node);
    R OnModule(ModuleSyntax node);
    R OnPublicExport(PublicExportSyntax node);
    R OnUseImport(UseImportSyntax node);
    R OnCodeBlock(CodeBlockSyntax node);

    R OnFunctionDeclaration(FunctionDeclarationSyntax node);
    R OnParameter(ParameterSyntax node);
    R OnArgument(ArgumentSyntax node);

    R OnType(TypeSyntax node);
    R OnTypeDeclaration(TypeDeclarationSyntax node);

    R OnTypeParameterGeneric(TypeParameterGenericSyntax node);
    R OnTypeParameterTemplate(TypeParameterTemplateSyntax node);
    R OnTypeParameterValue(TypeParameterValueSyntax node);
    R OnTypeArgument(TypeArgumentSyntax node);
    R OnTypeInitializerField(TypeInitializerFieldSyntax node);

    R OnMemberEnum(MemberEnumSyntax node);
    R OnMemberField(MemberFieldSyntax node);
    R OnMemberRule(MemberRuleSyntax node);

    R OnTypeMemberAccess(ExpressionMemberAccessSyntax node);

    R OnVariableDeclarationTyped(VariableDeclarationTypedSyntax node);
    R OnVariableDeclarationInferred(VariableDeclarationInferredSyntax node);

    R OnStatementAssignment(StatementAssignmentSyntax node);
    R OnStatementIf(StatementIfSyntax node);
    R OnStatementElseIf(StatementElseIfSyntax node);
    R OnStatementElse(StatementElseSyntax node);
    R OnStatementReturn(StatementReturnSyntax node);
    R OnStatementExpression(StatementExpressionSyntax node);

    R OnStatementLoop(StatementLoopSyntax node);

    R OnExpressionUnary(ExpressionUnarySyntax node);
    R OnExpressionBinary(ExpressionBinarySyntax node);
    R OnExpressionInvocation(ExpressionInvocationSyntax node);
    R OnExpressionTypeInitializer(ExpressionTypeInitializerSyntax node);
    R OnExpressionIdentifier(ExpressionIdentifierSyntax node);
    R OnExpressionLiteral(ExpressionLiteralSyntax node);
    R OnExpressionLiteralBool(ExpressionLiteralBoolSyntax node);
    R OnExpressionRange(ExpressionRangeSyntax node);

    R OnExpressionOperator(ExpressionOperatorSyntax node);

    R OnLiteralNumber(LiteralNumberSyntax node);
    R OnLiteralString(LiteralStringSyntax node);

    R OnName(NameSyntax node);
    R OnQualifiedName(QualifiedNameSyntax node);
}

#nullable disable
/// <summary>
/// A base class that walks the node hierarchy.
/// </summary>
/// <typeparam name="R">The return type of the VisitXxxx methods.</typeparam>
public abstract class SyntaxVisitor<R> : ISyntaxVisitor<R>
    where R : new()
{
    protected virtual R VisitChildren<T>(T node)
        where T : SyntaxNode
    {
        R result = Default;

        foreach (var child in node.ChildNodes)
        {
            var newResult = child.Accept(this);
            result = AggregateResult(result, newResult);
        }

        return result;
    }

    public virtual R Default => default;

    public virtual R AggregateResult(R aggregate, R newResult)
        => newResult;

    public virtual R OnCompilationUnit(CompilationUnitSyntax node)
        => VisitChildren(node);

    public virtual R OnModule(ModuleSyntax node)
        => VisitChildren(node);

    public virtual R OnPublicExport(PublicExportSyntax node)
        => VisitChildren(node);

    public virtual R OnUseImport(UseImportSyntax node)
        => VisitChildren(node);

    public virtual R OnCodeBlock(CodeBlockSyntax node)
        => VisitChildren(node);

    public virtual R OnFunctionDeclaration(FunctionDeclarationSyntax node)
        => VisitChildren(node);

    public virtual R OnParameter(ParameterSyntax node)
        => VisitChildren(node);

    public virtual R OnArgument(ArgumentSyntax node)
        => VisitChildren(node);

    public virtual R OnType(TypeSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeInitializerField(TypeInitializerFieldSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeDeclaration(TypeDeclarationSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeParameterGeneric(TypeParameterGenericSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeParameterTemplate(TypeParameterTemplateSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeParameterValue(TypeParameterValueSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeArgument(TypeArgumentSyntax node)
        => VisitChildren(node);

    public virtual R OnMemberEnum(MemberEnumSyntax node)
        => VisitChildren(node);

    public virtual R OnMemberField(MemberFieldSyntax node)
        => VisitChildren(node);

    public virtual R OnMemberRule(MemberRuleSyntax node)
        => VisitChildren(node);

    public virtual R OnTypeMemberAccess(ExpressionMemberAccessSyntax node)
        => VisitChildren(node);

    public virtual R OnVariableDeclarationTyped(VariableDeclarationTypedSyntax node)
        => VisitChildren(node);
    public virtual R OnVariableDeclarationInferred(VariableDeclarationInferredSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementAssignment(StatementAssignmentSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementIf(StatementIfSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementElseIf(StatementElseIfSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementElse(StatementElseSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementReturn(StatementReturnSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementExpression(StatementExpressionSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementLoop(StatementLoopSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionUnary(ExpressionUnarySyntax node)
        => VisitChildren(node);
    public virtual R OnExpressionBinary(ExpressionBinarySyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionInvocation(ExpressionInvocationSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionTypeInitializer(ExpressionTypeInitializerSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionIdentifier(ExpressionIdentifierSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionLiteral(ExpressionLiteralSyntax node)
        => VisitChildren(node);
    public virtual R OnExpressionLiteralBool(ExpressionLiteralBoolSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionRange(ExpressionRangeSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionOperator(ExpressionOperatorSyntax node)
        => VisitChildren(node);

    public virtual R OnLiteralNumber(LiteralNumberSyntax node)
        => VisitChildren(node);
    public virtual R OnLiteralString(LiteralStringSyntax node)
        => VisitChildren(node);

    public virtual R OnName(NameSyntax node)
        => VisitChildren(node);

    public virtual R OnQualifiedName(QualifiedNameSyntax node)
        => VisitChildren(node);
}
#nullable enable