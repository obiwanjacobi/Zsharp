using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranch : AstNode,
        IAstCodeBlockLine
    {
        public AstBranch(Statement_breakContext context)
            : base(AstNodeKind.Branch)
        {
            Context = context;
            BranchKind = AstBranchKind.ExitLoop;
        }

        public AstBranch(Statement_continueContext context)
            : base(AstNodeKind.Branch)
        {
            Context = context;
            BranchKind = AstBranchKind.ExitIteration;
        }

        protected AstBranch(AstBranchKind branchKind)
            : base(AstNodeKind.Branch)
        {
            BranchKind = branchKind;
        }

        public ParserRuleContext? Context { get; protected set; }

        public uint Indent { get; set; }

        public AstBranchKind BranchKind { get; }

        public bool IsExpression
            => BranchKind == AstBranchKind.ExitFunction || IsConditional;

        public AstBranchExpression? ToExpression()
            => IsExpression ? (AstBranchExpression)this : null;

        public bool HasCode
            => BranchKind == AstBranchKind.Conditional;

        public bool IsConditional
            => BranchKind == AstBranchKind.Conditional;

        public AstBranchConditional? ToConditional()
            => IsConditional ? (AstBranchConditional)this : null;

        public override void Accept(AstVisitor visitor)
            => visitor.VisitBranch(this);
    }
}