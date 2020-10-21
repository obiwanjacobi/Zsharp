using System;

namespace Zsharp.AST
{
    public interface IAstExpressionSite
    {
        AstExpression? Expression { get; }
        bool SetExpression(AstExpression expression);

        public void ThrowIfExpressionNotSet()
            => _ = Expression ?? throw new InvalidOperationException("Expression is not set.");
    }
}
