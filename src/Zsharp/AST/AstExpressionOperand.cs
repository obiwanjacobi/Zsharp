using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode, IAstTypeReferenceSite
    {
        private readonly Literal_boolContext? _litBoolCtx;

        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeType.Operand)
        {
            Expression = expr;
            expr.SetParent(this);
        }

        public AstExpressionOperand(AstNumeric num)
            : base(AstNodeType.Operand)
        {
            Numeric = num;
            num.SetParent(this);
        }

        public AstExpressionOperand(AstVariableReference variable)
            : base(AstNodeType.Operand)
        {
            VariableReference = variable;
            variable.SetParent(this);
        }

        public AstExpressionOperand(AstFunctionReference function)
            : base(AstNodeType.Operand)
        {
            FunctionReference = function;
            function.SetParent(this);
        }

        public AstExpressionOperand(Literal_boolContext context)
            : base(AstNodeType.Operand)
        {
            _litBoolCtx = context;
        }

        public ParserRuleContext? Context
        {
            get
            {
                if (_litBoolCtx != null)
                    return _litBoolCtx;
                return null;
            }
        }

        public AstExpression? Expression { get; }

        public AstNumeric? Numeric { get; }

        public AstVariableReference? VariableReference { get; }

        public AstFunctionReference? FunctionReference { get; }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => Ast.SafeSet(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitExpressionOperand(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
            Numeric?.Accept(visitor);
            VariableReference?.Accept(visitor);
            FunctionReference?.Accept(visitor);
        }
    }
}