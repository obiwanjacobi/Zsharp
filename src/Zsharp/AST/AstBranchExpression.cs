using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchExpression : AstBranch,
        IAstExpressionSite
    {
        public AstBranchExpression(Statement_returnContext context)
            : base(AstBranchKind.ExitFunction)
        {
            Context = context;
        }

        protected AstBranchExpression(AstBranchKind branchKind)
            : base(branchKind)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool HasExpression => _expression is not null;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitBranchExpression(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}
