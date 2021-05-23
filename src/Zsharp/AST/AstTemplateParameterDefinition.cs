using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter,
        IAstIdentifierSite, IAstSymbolEntrySite
    {
        public AstTemplateParameterDefinition(Template_param_anyContext context)
            : base(context)
        { }

        protected AstTemplateParameterDefinition()
            : base(null)
        { }

        public virtual bool IsIntrinsic => false;

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterDefinition(this);
    }
}