using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchExpression : AstBranch,
        IAstCodeBlockSite, IAstExpressionSite
    {
        public AstBranchExpression(Statement_returnContext context)
            : base(AstBranchKind.ExitFunction)
        {
            Context = context;
        }

        public AstBranchExpression(Statement_loop_infiniteContext context)
            : base(AstBranchKind.Loop)
        {
            Context = context;

            // loop true
            var trueExpression = new AstExpression(context);
            trueExpression.Add(
                new AstExpressionOperand(
                    new AstLiteralBoolean(true)));

            this.SetExpression(trueExpression);
        }

        public AstBranchExpression(Statement_loop_iterationContext context)
            : base(AstBranchKind.Loop)
        {
            Context = context;
        }

        public AstBranchExpression(Statement_loop_whileContext context)
            : base(AstBranchKind.Loop)
        {
            Context = context;
        }

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
