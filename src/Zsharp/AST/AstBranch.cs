using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstBranch : AstNode,
        IAstCodeBlockLine
    {
        internal AstBranch(ParserRuleContext context, AstBranchKind branchKind)
            : base(AstNodeKind.Branch)
        {
            Context = context;
            BranchKind = branchKind;
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