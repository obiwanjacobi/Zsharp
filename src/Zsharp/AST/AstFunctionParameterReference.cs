using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterReference : AstFunctionParameter, IAstExpressionSite
    {
        public AstFunctionParameterReference(Function_param_useContext paramCtx)
        {
            Context = paramCtx;
        }

        public ParserRuleContext Context { get; }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression expression) => this.SafeSetParent(ref _expression, expression);

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionParameterReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
        }
    }
}
