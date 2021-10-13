using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeFieldInitialization : AstTypeFieldReference,
        IAstExpressionSite
    {
        internal AstTypeFieldInitialization(ParserRuleContext context)
            : base(context)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeFieldInitialization(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}