using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstFunctionParameterReference : AstFunctionParameter,
        IAstExpressionSite
    {
        internal AstFunctionParameterReference(ParserRuleContext context)
            : base(context)
        { }

        public AstFunctionParameterReference(AstExpression expression)
            : base(null)
        {
            TrySetExpression(expression);
        }

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
