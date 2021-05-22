using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchExpression : AstBranch,
        IAstExpressionSite
    {
        public AstBranchExpression(Statement_returnContext context)
            : base(AstBranchType.ExitFunction)
        {
            Context = context;
        }

        protected AstBranchExpression(AstBranchType branchType)
            : base(branchType)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool HasExpression => _expression != null;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitBranchExpression(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}
