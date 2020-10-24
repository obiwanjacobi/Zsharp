using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstExpression : AstNode, IAstTypeReferenceSite
    {
        private readonly Expression_arithmeticContext? _arithmeticCtx;
        private readonly Expression_logicContext? _logicCtx;
        private readonly Expression_comparisonContext? _comparisonCtx;
        private readonly Expression_valueContext? _valueCtx;

        public AstExpression(Expression_arithmeticContext context)
            : base(AstNodeType.Expression)
        {
            _arithmeticCtx = context;
        }

        public AstExpression(Expression_logicContext context)
            : base(AstNodeType.Expression)
        {
            _logicCtx = context;
        }

        public AstExpression(Expression_comparisonContext context)
            : base(AstNodeType.Expression)
        {
            _comparisonCtx = context;
        }

        public AstExpression(Expression_valueContext context)
            : base(AstNodeType.Expression)
        {
            _valueCtx = context;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitExpression(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            _rhs?.Accept(visitor);
            _lhs?.Accept(visitor);
        }

        private AstExpressionOperand? _lhs;
        public AstExpressionOperand? LHS => _lhs;

        private AstExpressionOperand? _rhs;
        public AstExpressionOperand? RHS => _rhs;

        public AstExpressionOperator Operator { get; set; }

        public int Precedence => (int)(Operator & AstExpressionOperator.MaskPrecedence) >> 4;

        public bool IsOperator(AstExpressionOperator op) => (Operator & op) > 0;

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

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
                bool success = op.TrySetParent(this);
                Ast.Guard(success, "SetParent failed.");
                _rhs = op;
                return true;
            }

            if (_lhs == null &&
                !IsOperator(AstExpressionOperator.MaskUnary))
            {
                bool success = op.TrySetParent(this);
                Ast.Guard(success, "SetParent failed.");
                _lhs = op;
                return true;
            }

            return false;
        }
    }
}