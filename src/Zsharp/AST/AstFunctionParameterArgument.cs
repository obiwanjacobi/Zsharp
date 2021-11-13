using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstFunctionParameterArgument : AstFunctionParameter,
        IAstExpressionSite
    {
        internal AstFunctionParameterArgument(ParserRuleContext context)
            : base(context)
        { }

        public AstFunctionParameterArgument(AstExpression expression)
            : base(null)
        {
            TrySetExpression(expression);
        }

        public bool HasExpression => _expression is not null;

        private AstExpression? _expression;
        public AstExpression Expression
            => _expression ?? throw new InternalErrorException("Expression was not set.");

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionParameterArgument(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            if (HasExpression)
                Expression.Accept(visitor);
        }
    }
}
