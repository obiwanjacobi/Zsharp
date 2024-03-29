using System;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode,
        IAstTypeReferenceSite, IAstExpressionSite
    {
        public AstExpressionOperand(AstNode node)
            : base(AstNodeKind.Operand)
        {
            Ast.Guard(node, "Expression Operand created with null.");

            _expression = node as AstExpression;
            LiteralBoolean = node as AstLiteralBoolean;
            LiteralNumeric = node as AstLiteralNumeric;
            LiteralString = node as AstLiteralString;
            VariableReference = node as AstVariableReference;
            FunctionReference = node as AstFunctionReference;
            FieldReference = node as AstTypeFieldReference;

            if (!HasExpression &&
                LiteralBoolean is null &&
                LiteralNumeric is null &&
                LiteralString is null &&
                VariableReference is null &&
                FunctionReference is null &&
                FieldReference is null)
            {
                throw new InternalErrorException(
                    $"Node type {node.GetType().Name} is not an expression operand.");
            }

            node.SetParent(this);

            if (node is IAstTypeReferenceSite typeRefSite &&
                typeRefSite.HasTypeReference)
                this.SetTypeReference(typeRefSite.TypeReference.MakeCopy());
        }

        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeKind.Operand)
        {
            TrySetExpression(expr);
        }

        public bool HasExpression => _expression is not null;

        private AstExpression? _expression;
        public AstExpression Expression
            => _expression ?? throw new InternalErrorException("Expression was not set.");

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

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitExpressionOperand(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            if (HasExpression)
                Expression.Accept(visitor);
            LiteralBoolean?.Accept(visitor);
            LiteralNumeric?.Accept(visitor);
            LiteralString?.Accept(visitor);
            VariableReference?.Accept(visitor);
            FunctionReference?.Accept(visitor);
            FieldReference?.Accept(visitor);
        }
    }
}