using System;

namespace Zsharp.AST
{
    public interface IAstExpressionSite
    {
        AstExpression? Expression { get; }
        bool TrySetExpression(AstExpression expression);

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        public void ThrowIfExpressionNotSet()
            => _ = Expression ?? throw new InvalidOperationException("Expression is not set.");
    }
}
