using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchConditional : AstBranchExpression, IAstCodeBlockSite
    {
        private readonly Statement_ifContext? _ifCtx;
        private readonly Statement_elseContext? _elseCtx;
        private readonly Statement_elseifContext? _elseifCtx;

        public AstBranchConditional(Statement_ifContext ctx)
            : base(AstBranchType.Conditional)
        {
            _ifCtx = ctx;
        }
        public AstBranchConditional(Statement_elseContext ctx)
            : base(AstBranchType.Conditional)
        {
            _elseCtx = ctx;
        }
        public AstBranchConditional(Statement_elseifContext ctx)
            : base(AstBranchType.Conditional)
        {
            _elseifCtx = ctx;
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock { get { return _codeBlock; } }

        public bool SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                codeBlock.Indent = Indent + 1;
                return true;
            }
            return false;
        }

        private AstBranchConditional? _subBranch;
        public AstBranchConditional? SubBranch { get { return _subBranch; } }

        public bool HasSubBranch { get { return _subBranch != null; } }

        public bool AddSubBranch(AstBranchConditional subBranch)
        {
            if (this.SafeSetParent(ref _subBranch, subBranch))
            {
                _subBranch!.Indent = Indent;
                return true;
            }
            return false;
        }

        public AstBranchConditional LastSubBranch()
        {
            return HasSubBranch ? _subBranch!.LastSubBranch() : this;
        }

        public override void Accept(AstVisitor visitor)
        {
            base.Accept(visitor);
            visitor.VisitBranchConditional(this);
        }
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
