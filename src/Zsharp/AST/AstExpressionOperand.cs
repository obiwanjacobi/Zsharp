using System;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode,
        IAstTypeReferenceSite, IAstExpressionSite
    {
        public AstExpressionOperand(AstNode node)
            : base(AstNodeType.Operand)
        {
            Ast.Guard(node, "Expression Operand created with null.");

            _expression = node as AstExpression;
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
                this.SetTypeReference(typeRefSite.TypeReference.MakeProxy());
        }

        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeType.Operand)
        {
            TrySetExpression(expr);
        }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        public AstLiteralBoolean? LiteralBoolean { get; }

        public AstLiteralNumeric? LiteralNumeric { get; }

        public AstLiteralString? LiteralString { get; }

        public AstVariableReference? VariableReference { get; }

        public AstFunctionReference? FunctionReference { get; }

        public AstTypeFieldReference? FieldReference { get; }

        private AstTypeReference? _typeRef;

        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => Ast.SafeSet(ref _typeRef, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitExpressionOperand(this);

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