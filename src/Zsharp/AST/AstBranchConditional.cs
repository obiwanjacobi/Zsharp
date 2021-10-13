using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstBranchConditional : AstBranchExpression
    {
        internal AstBranchConditional(ParserRuleContext context)
            : base(AstBranchKind.Conditional)
        {
            Context = context;
        }

        private AstBranchConditional? _subBranch;
        public AstBranchConditional? SubBranch => _subBranch;

        public bool HasSubBranch => _subBranch is not null;

        public bool TryAddSubBranch(AstBranchConditional? subBranch)
        {
            if (this.SafeSetParent(ref _subBranch, subBranch))
            {
                _subBranch!.Indent = Indent;
                return true;
            }
            return false;
        }

        public void AddSubBranch(AstBranchConditional subBranch)
        {
            if (!TryAddSubBranch(subBranch))
                throw new InternalErrorException(
                    "SubBranch is already set or null.");
        }

        public AstBranchConditional LastSubBranch() => HasSubBranch ? _subBranch!.LastSubBranch() : this;

        public override void Accept(AstVisitor visitor) => visitor.VisitBranchConditional(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            if (HasSubBranch)
            {
                _subBranch!.Accept(visitor);
            }
        }
    }
}
