namespace Maja.Compiler.Syntax;

public interface ISyntaxVisitable
{
    R Accept<R>(ISyntaxVisitor<R> visitor);
}

public interface ISyntaxVisitor<R>
{
    R OnCompilationUnit(CompilationUnitSyntax node);
    R OnPublicExport(PublicExportSyntax node);
    R OnUseImport(UseImportSyntax node);

    R OnCodeBlock(CodeBlockSyntax node);

    R OnFunctionDeclaration(FunctionDelcarationSyntax node);
    R OnParameter(ParameterSyntax node);

    R OnType(TypeSyntax node);

    R OnVariableDeclaration(VariableDeclarationSyntax node);

    R OnStatementReturn(StatementReturnSyntax node);

    R OnExpression(ExpressionSyntax node);
    R OnExpressionLiteral(ExpressionLiteralSyntax node);
    R OnExpressionLiteralBool(ExpressionLiteralBoolSyntax node);

    R OnName(NameSyntax node);
}

#nullable disable
public abstract class SyntaxVisitor<R> : ISyntaxVisitor<R>
    where R: new()
{
    protected virtual R VisitChildren<T>(T node)
        where T: SyntaxNode
    {
        R result = default;

        foreach (var child in node.Children)
        {
            var newResult = child.Accept(this);
            result = AggregateResult(result, newResult);
        }

        return result;
    }

    protected virtual R AggregateResult(R aggregate, R newResult)
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

    public virtual R OnType(TypeSyntax node)
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

    public virtual R OnName(NameSyntax node)
        => VisitChildren(node);
}
#nullable enable