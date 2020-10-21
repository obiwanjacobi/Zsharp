using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranch : AstCodeBlockItem
    {
        private readonly Statement_returnContext? _returnCtx;
        private readonly Statement_breakContext? _breakCtx;
        private readonly Statement_continueContext? _continueCtx;

        public AstBranch(Statement_returnContext context)
            : base(AstNodeType.Branch)
        {
            _returnCtx = context;
            BranchType = AstBranchType.ExitFunction;
        }

        public AstBranch(Statement_breakContext context)
            : base(AstNodeType.Branch)
        {
            _breakCtx = context;
            BranchType = AstBranchType.ExitLoop;
        }

        public AstBranch(Statement_continueContext context)
            : base(AstNodeType.Branch)
        {
            _continueCtx = context;
            BranchType = AstBranchType.ExitIteration;
        }

        protected AstBranch(AstBranchType branchType)
            : base(AstNodeType.Branch)
        {
            BranchType = branchType;
        }

        public AstBranchType BranchType { get; }

        public bool IsExpression => BranchType == AstBranchType.ExitFunction || IsConditional;

        public AstBranchExpression? ToExpression() => IsExpression ? (AstBranchExpression)this : null;

        public bool HasCode => BranchType == AstBranchType.Conditional;

        public bool IsConditional => BranchType == AstBranchType.Conditional;

        public AstBranchConditional? ToConditional() => IsConditional ? (AstBranchConditional)this : null;

        public override void Accept(AstVisitor visitor) => visitor.VisitBranch(this);
    }
}