using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchConditional : AstBranchExpression, IAstCodeBlockSite
    {
        private readonly Statement_ifContext? _ifCtx;
        private readonly Statement_elseContext? _elseCtx;
        private readonly Statement_elseifContext? _elseifCtx;

        public AstBranchConditional(Statement_ifContext context)
            : base(AstBranchType.Conditional)
        {
            _ifCtx = context;
        }
        public AstBranchConditional(Statement_elseContext context)
            : base(AstBranchType.Conditional)
        {
            _elseCtx = context;
        }
        public AstBranchConditional(Statement_elseifContext context)
            : base(AstBranchType.Conditional)
        {
            _elseifCtx = context;
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                codeBlock.Indent = Indent + 1;
                return true;
            }
            return false;
        }

        public void SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (!TrySetCodeBlock(codeBlock))
                throw new InvalidOperationException(
                    "CodeBlock is already set or null.");
        }

        private AstBranchConditional? _subBranch;
        public AstBranchConditional? SubBranch => _subBranch;

        public bool HasSubBranch => _subBranch != null;

        public bool TryAddSubBranch(AstBranchConditional subBranch)
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
                throw new InvalidOperationException(
                    "SubBranch is already set or null.");
        }

        public AstBranchConditional LastSubBranch() => HasSubBranch ? _subBranch!.LastSubBranch() : this;

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
