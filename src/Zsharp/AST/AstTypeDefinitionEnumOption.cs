using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnumOption : AstTypeFieldDefinition,
        IAstExpressionSite
    {
        internal AstTypeDefinitionEnumOption(ParserRuleContext context)
            : base(context, AstNodeKind.EnumOption)
        { }

        public bool HasExpression => _expression is not null;

        private AstExpression? _expression;
        public AstExpression Expression
            => _expression ?? throw new InternalErrorException("Expression was not set.");

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.EnumOption);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionEnumOption(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}