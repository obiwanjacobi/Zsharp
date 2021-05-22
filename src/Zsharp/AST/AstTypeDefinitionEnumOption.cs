using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnumOption : AstTypeFieldDefinition,
        IAstExpressionSite, IAstSymbolEntrySite
    {
        public AstTypeDefinitionEnumOption(Enum_option_defContext context)
            : base(AstNodeType.EnumOption)
        {
            Context = context;
        }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression? expression)
            => this.SafeSetParent(ref _expression, expression);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public bool TryResolve()
        {
            return _symbol?.Definition == this;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionEnumOption(this);

        public override void VisitChildren(AstVisitor visitor)
            => Expression?.Accept(visitor);
    }
}