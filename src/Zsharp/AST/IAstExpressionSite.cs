namespace Zsharp.AST
{
    public interface IAstExpressionSite
    {
        AstExpression? Expression { get; }
        bool SetExpression(AstExpression expression);
    }
}
