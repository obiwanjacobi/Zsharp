using System;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode, IAstTypeReferenceSite
    {
        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeType.Operand)
        {
            Expression = expr;
            expr.SetParent(this);
        }

        public AstExpressionOperand(AstLiteralBoolean litBool)
            : base(AstNodeType.Operand)
        {
            LiteralBoolean = litBool;
            litBool.SetParent(this);
        }

        public AstExpressionOperand(AstLiteralNumeric num)
            : base(AstNodeType.Operand)
        {
            LiteralNumeric = num;
            num.SetParent(this);
        }

        public AstExpressionOperand(AstLiteralString litStr)
            : base(AstNodeType.Operand)
        {
            LiteralString = litStr;
            litStr.SetParent(this);
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

        public AstExpression? Expression { get; }

        public AstLiteralBoolean? LiteralBoolean { get; }

        public AstLiteralNumeric? LiteralNumeric { get; }

        public AstLiteralString? LiteralString { get; }

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
            LiteralBoolean?.Accept(visitor);
            LiteralNumeric?.Accept(visitor);
            LiteralString?.Accept(visitor);
            VariableReference?.Accept(visitor);
            FunctionReference?.Accept(visitor);
        }
    }
}