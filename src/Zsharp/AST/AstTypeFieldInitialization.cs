using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeFieldInitialization : AstTypeFieldReference,
        IAstExpressionSite
    {
        public AstTypeFieldInitialization(Struct_field_initContext context)
            : base(context)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public AstTypeFieldDefinition FieldDefinition => Symbol!.DefinitionAs<AstTypeFieldDefinition>()!;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeFieldInitialization(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
        }
    }
}