using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnumOption : AstTypeFieldDefinition,
        IAstExpressionSite
    {
        public AstTypeDefinitionEnumOption(Enum_option_defContext context)
            : base(context, AstNodeType.EnumOption)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionEnumOption(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}