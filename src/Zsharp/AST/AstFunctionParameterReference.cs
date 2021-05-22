using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterReference : AstFunctionParameter,
        IAstExpressionSite
    {
        public AstFunctionParameterReference(Function_param_useContext paramCtx)
        {
            Context = paramCtx;
        }

        public AstFunctionParameterReference(AstExpression expression)
        {
            TrySetExpression(expression);
        }

        public ParserRuleContext? Context { get; }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionParameterReference(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            Expression?.Accept(visitor);
        }
    }
}
