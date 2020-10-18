using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchExpression : AstBranch, IAstExpressionSite
    {
        public AstBranchExpression(Statement_returnContext context)
            : base(context)
        { }

        protected AstBranchExpression(AstBranchType branchType)
            : base(branchType)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression { get { return _expression; } }

        public bool HasExpression
        {
            get { return _expression != null; }
        }

        public bool SetExpression(AstExpression expression)
        {
            return this.SafeSetParent(ref _expression, expression);
        }

        public override void Accept(AstVisitor visitor)
        {
            base.Accept(visitor);
            visitor.VisitBranchExpression(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
        }
    }
}
