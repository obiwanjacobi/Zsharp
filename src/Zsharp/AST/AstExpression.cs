using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstExpression : AstNode,
        IAstTypeReferenceSite
    {
        public AstExpression(AstExpressionOperand operand)
            : base(AstNodeKind.Expression)
        {
            TrySetRHS(operand);
        }

        internal AstExpression(ParserRuleContext context)
                : base(AstNodeKind.Expression)
        {
            Context = context;
        }

        public ParserRuleContext? Context { get; }

        public override void Accept(AstVisitor visitor) => visitor.VisitExpression(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            _rhs?.Accept(visitor);
            _lhs?.Accept(visitor);
            _typeRef?.Accept(visitor);
        }

        private AstExpressionOperand? _lhs;
        public AstExpressionOperand? LHS => _lhs;

        private AstExpressionOperand? _rhs;
        public AstExpressionOperand? RHS => _rhs;

        protected bool TrySetLHS(AstExpressionOperand operand)
            => this.SafeSetParent(ref _lhs, operand);

        protected bool TrySetRHS(AstExpressionOperand operand)
            => this.SafeSetParent(ref _rhs, operand);

        public AstExpressionOperator Operator { get; set; }

        public int Precedence => (int)(Operator & AstExpressionOperator.MaskPrecedence) >> 4;

        public bool IsOperator(AstExpressionOperator op) => (Operator & op) > 0;

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        public bool Add(AstExpressionOperand op)
        {
            if (_rhs is null)
            {
                op.SetParent(this);
                _rhs = op;
                return true;
            }

            if (_lhs is null &&
                !IsOperator(AstExpressionOperator.MaskUnary))
            {
                op.SetParent(this);
                _lhs = op;
                return true;
            }

            return false;
        }
    }
}