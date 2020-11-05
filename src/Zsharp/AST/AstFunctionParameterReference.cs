using Antlr4.Runtime;
using System;
using Zsharp.Parser;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterReference : AstFunctionParameter, IAstExpressionSite
    {
        private readonly Function_param_useContext _paramCtx;

        public AstFunctionParameterReference(ZsharpParser.Function_param_useContext paramCtx)
        {
            _paramCtx = paramCtx;
        }

        public ParserRuleContext Context => _paramCtx;

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
