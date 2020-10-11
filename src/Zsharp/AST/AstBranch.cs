using static ZsharpParser;

namespace Zlang.NET.AST
{
    public enum AstBranchType
    {
        NotSet,
        Conditional,        // if-else
        ExitIteration,      // continue
        ExitLoop,           // break
        ExitFunction,       // return
        ExitProgram,        // abort
    }

    public class AstBranch : AstCodeBlockItem
    {
        private readonly Statement_returnContext? _returnCtx;
        private readonly Statement_breakContext? _breakCtx;
        private readonly Statement_continueContext? _continueCtx;

        public AstBranch(Statement_returnContext ctx)
            : base(AstNodeType.Branch)
        {
            _returnCtx = ctx;
            BranchType = AstBranchType.ExitFunction;
        }
        public AstBranch(Statement_breakContext ctx)
            : base(AstNodeType.Branch)
        {
            _breakCtx = ctx;
            BranchType = AstBranchType.ExitLoop;
        }
        public AstBranch(Statement_continueContext ctx)
            : base(AstNodeType.Branch)
        {
            _continueCtx = ctx;
            BranchType = AstBranchType.ExitIteration;
        }
        protected AstBranch(AstBranchType branchType)
            : base(AstNodeType.Branch)
        {
            BranchType = branchType;
        }

        public AstBranchType BranchType { get; }

        public bool IsExpression
        {
            get { return BranchType == AstBranchType.ExitFunction || IsConditional; }
        }
        public AstBranchExpression? ToExpression()
        {
            return IsExpression ? (AstBranchExpression)this : null;
        }

        public bool HasCode
        {
            get { return BranchType == AstBranchType.Conditional; }
        }
        public bool IsConditional
        {
            get { return BranchType == AstBranchType.Conditional; }
        }
        public AstBranchConditional? ToConditional()
        {
            return IsConditional ? (AstBranchConditional)this : null;
        }

        public override void Accept(AstVisitor visitor)
        {
            base.Accept(visitor);
            visitor.VisitBranch(this);
        }
    }

    public class AstBranchExpression : AstBranch, IAstExpressionSite
    {
        public AstBranchExpression(Statement_returnContext ctx)
            : base(ctx)
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
        public AstBranchConditional()
            : base(AstBranchType.Conditional)
        { }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock { get { return _codeBlock; } }
        public bool HasCodeBlock { get { return _codeBlock != null; } }

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