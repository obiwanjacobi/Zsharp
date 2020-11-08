using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranch : AstCodeBlockItem
    {
        public AstBranch(Statement_breakContext context)
            : base(AstNodeType.Branch)
        {
            Context = context;
            BranchType = AstBranchType.ExitLoop;
        }

        public AstBranch(Statement_continueContext context)
            : base(AstNodeType.Branch)
        {
            Context = context;
            BranchType = AstBranchType.ExitIteration;
        }

        protected AstBranch(AstBranchType branchType)
            : base(AstNodeType.Branch)
        {
            BranchType = branchType;
        }

        public ParserRuleContext? Context { get; protected set; }

        public AstBranchType BranchType { get; }

        public bool IsExpression => BranchType == AstBranchType.ExitFunction || IsConditional;

        public AstBranchExpression? ToExpression() => IsExpression ? (AstBranchExpression)this : null;

        public bool HasCode => BranchType == AstBranchType.Conditional;

        public bool IsConditional => BranchType == AstBranchType.Conditional;

        public AstBranchConditional? ToConditional() => IsConditional ? (AstBranchConditional)this : null;

        public override void Accept(AstVisitor visitor) => visitor.VisitBranch(this);
    }
}