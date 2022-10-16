namespace Maja.Compiler.Syntax;

public interface ISyntaxVisitor<R>
{
    R Default { get; }
    R AggregateResult(R aggregate, R newResult);

    R OnCompilationUnit(CompilationUnitSyntax node);
    R OnPublicExport(PublicExportSyntax node);
    R OnUseImport(UseImportSyntax node);

    R OnCodeBlock(CodeBlockSyntax node);

    R OnFunctionDeclaration(FunctionDelcarationSyntax node);
    R OnParameter(ParameterSyntax node);
    R OnArgument(ArgumentSyntax node);

    R OnType(TypeSyntax node);
    R OnTypeDeclaration(TypeDeclarationSyntax node);

    R OnTypeParameterGeneric(TypeParameterGenericSyntax node);
    R OnTypeParameterTemplate(TypeParameterTemplateSyntax node);
    R OnTypeParameterValue(TypeParameterValueSyntax node);
    R OnTypeArgument(TypeArgumentSyntax node);

    R OnMemberEnum(MemberEnumSyntax node);
    R OnMemberField(MemberFieldSyntax node);
    R OnMemberRule(MemberRuleSyntax node);

    R OnVariableDeclaration(VariableDeclarationSyntax node);

    R OnStatementReturn(StatementReturnSyntax node);

    R OnExpression(ExpressionSyntax node);
    R OnExpressionLiteral(ExpressionLiteralSyntax node);
    R OnExpressionLiteralBool(ExpressionLiteralBoolSyntax node);

    R OnExpressionOperator(ExpressionOperatorSyntax node);

    R OnLiteralNumber(LiteralNumberSyntax node);
    R OnLiteralString(LiteralStringSyntax node);

    R OnName(NameSyntax node);
}

#nullable disable
public abstract class SyntaxVisitor<R> : ISyntaxVisitor<R>
    where R: new()
{
    protected virtual R VisitChildren<T>(T node)
        where T: SyntaxNode
    {
        R result = Default;

        foreach (var child in node.Children)
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

    public virtual R OnPublicExport(PublicExportSyntax node)
        => VisitChildren(node);

    public virtual R OnUseImport(UseImportSyntax node)
        => VisitChildren(node);

    public virtual R OnCodeBlock(CodeBlockSyntax node)
        => VisitChildren(node);

    public virtual R OnFunctionDeclaration(FunctionDelcarationSyntax node)
        => VisitChildren(node);

    public virtual R OnParameter(ParameterSyntax node)
        => VisitChildren(node);

    public virtual R OnArgument(ArgumentSyntax node)
        => VisitChildren(node);

    public virtual R OnType(TypeSyntax node)
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

    public virtual R OnVariableDeclaration(VariableDeclarationSyntax node)
        => VisitChildren(node);

    public virtual R OnStatementReturn(StatementReturnSyntax node)
        => VisitChildren(node);

    public virtual R OnExpression(ExpressionSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionLiteral(ExpressionLiteralSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionLiteralBool(ExpressionLiteralBoolSyntax node)
        => VisitChildren(node);

    public virtual R OnExpressionOperator(ExpressionOperatorSyntax node)
        => VisitChildren(node);

    public virtual R OnLiteralNumber(LiteralNumberSyntax node)
        => VisitChildren(node);
    public virtual R OnLiteralString(LiteralStringSyntax node)
        => VisitChildren(node);

    public virtual R OnName(NameSyntax node)
        => VisitChildren(node);
}
#nullable enable