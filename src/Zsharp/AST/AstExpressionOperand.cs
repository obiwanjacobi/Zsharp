using System;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode, IAstTypeReferenceSite
    {
        public AstExpressionOperand(AstNode node)
            : base(AstNodeType.Operand)
        {
            Ast.Guard(node, "Expression Operand created with null.");

            Expression = node as AstExpression;
            LiteralBoolean = node as AstLiteralBoolean;
            LiteralNumeric = node as AstLiteralNumeric;
            LiteralString = node as AstLiteralString;
            VariableReference = node as AstVariableReference;
            FunctionReference = node as AstFunctionReference;
            FieldReference = node as AstTypeFieldReference;

            if (Expression == null &&
                LiteralBoolean == null &&
                LiteralNumeric == null &&
                LiteralString == null &&
                VariableReference == null &&
                FunctionReference == null &&
                FieldReference == null)
            {
                throw new ArgumentException(
                    $"Node type {node.GetType().Name} is not an expression operand.");
            }

            node.SetParent(this);

            if (node is IAstTypeReferenceSite typeRefSite &&
                typeRefSite.TypeReference != null)
                SetTypeReference(new AstTypeReference(typeRefSite.TypeReference));
        }

        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeType.Operand)
        {
            Expression = expr;
            expr.SetParent(this);
        }

        public AstExpression? Expression { get; }

        public AstLiteralBoolean? LiteralBoolean { get; }

        public AstLiteralNumeric? LiteralNumeric { get; }

        public AstLiteralString? LiteralString { get; }

        public AstVariableReference? VariableReference { get; }

        public AstFunctionReference? FunctionReference { get; }

        public AstTypeFieldReference? FieldReference { get; }

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
            FieldReference?.Accept(visitor);
        }
    }
}