using Antlr4.Runtime;
using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstExpression : AstNode, IAstTypeReferenceSite
    {
        private readonly Expression_arithmeticContext? _arithmeticCtx;
        private readonly Expression_logicContext? _logicCtx;
        private readonly Expression_comparisonContext? _comparisonCtx;
        private readonly Expression_valueContext? _valueCtx;

        public AstExpression(Expression_arithmeticContext ctx)
            : base(AstNodeType.Expression)
        {
            _arithmeticCtx = ctx;
        }

        public AstExpression(Expression_logicContext ctx)
            : base(AstNodeType.Expression)
        {
            _logicCtx = ctx;
        }

        public AstExpression(Expression_comparisonContext ctx)
            : base(AstNodeType.Expression)
        {
            _comparisonCtx = ctx;
        }

        public AstExpression(Expression_valueContext ctx)
            : base(AstNodeType.Expression)
        {
            _valueCtx = ctx;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitExpression(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            if (_rhs != null)
            {
                _rhs.Accept(visitor);
            }
            if (_lhs != null)
            {
                _lhs.Accept(visitor);
            }
        }

        private AstExpressionOperand? _lhs;
        public AstExpressionOperand? LHS => _lhs;

        private AstExpressionOperand? _rhs;
        public AstExpressionOperand? RHS => _rhs;

        public AstExpressionOperator Operator { get; set; }

        public int Precedence
        {
            get { return (int)(Operator & AstExpressionOperator.MaskPrecedence) >> 4; }
        }

        public bool IsOperator(AstExpressionOperator op)
        {
            return (Operator & op) > 0;
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public ParserRuleContext? getContext()
        {
            if (_arithmeticCtx != null)
                return _arithmeticCtx;
            if (_logicCtx != null)
                return _logicCtx;
            if (_comparisonCtx != null)
                return _comparisonCtx;
            if (_valueCtx != null)
                return _valueCtx;
            return null;
        }

        public bool Add(AstExpressionOperand op)
        {
            if (_rhs == null)
            {
                bool success = op.SetParent(this);
                Ast.Guard(success, "SetParent failed.");
                _rhs = op;
                return true;
            }

            if (_lhs == null &&
                !IsOperator(AstExpressionOperator.MaskUnary))
            {
                bool success = op.SetParent(this);
                Ast.Guard(success, "SetParent failed.");
                _lhs = op;
                return true;
            }

            return false;
        }

        public bool SetTypeReference(AstTypeReference typeRef)
        {
            return this.SafeSetParent(ref _typeRef, typeRef);
        }
    }
}