using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstExpressionRange : AstExpression
    {
        public AstExpressionRange(RangeContext context)
            : base(context)
        { }

        public AstExpressionOperand? Begin => base.LHS;

        public AstExpressionOperand? End => base.RHS;

        private AstExpressionOperand? _step;
        public AstExpressionOperand? Step => _step;

        public bool TrySetBegin(AstExpressionOperand operand)
            => base.TrySetLHS(operand);

        public void SetBegin(AstExpressionOperand operand)
        {
            if (!TrySetBegin(operand))
                throw new InternalErrorException("Begin Expression Operand is already set or null.");
        }

        public bool TrySetEnd(AstExpressionOperand operand)
            => base.TrySetRHS(operand);

        public void SetEnd(AstExpressionOperand operand)
        {
            if (!TrySetEnd(operand))
                throw new InternalErrorException("End Expression Operand is already set or null.");
        }

        public bool TrySetStep(AstExpressionOperand operand)
            => this.SafeSetParent(ref _step, operand);

        public void SetStep(AstExpressionOperand operand)
        {
            if (!TrySetStep(operand))
                throw new InternalErrorException("Step Expression Operand is already set or null.");
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitExpressionRange(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            Begin?.Accept(visitor);
            End?.Accept(visitor);
            Step?.Accept(visitor);
        }
    }
}
