namespace Zsharp.AST
{
    public interface IAstExpressionSite
    {
        AstExpression? Expression { get; }
        bool TrySetExpression(AstExpression? expression);
    }

    public static class AstExpressionSiteExtensions
    {
        public static void SetExpression(this IAstExpressionSite expressionSite, AstExpression expression)
        {
            if (!expressionSite.TrySetExpression(expression))
                throw new ZsharpException(
                    "Expression is already set or null.");
        }
    }
}
