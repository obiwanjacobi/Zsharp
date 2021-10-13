using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstBranchExpression : AstBranch,
        IAstCodeBlockSite, IAstExpressionSite
    {
        internal AstBranchExpression(ParserRuleContext context, AstBranchKind branchKind)
            : base(context, branchKind)
        { }

        protected AstBranchExpression(AstBranchKind branchKind)
            : base(branchKind)
        { }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                codeBlock!.Indent = Indent + 1;
                return true;
            }
            return false;
        }

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
