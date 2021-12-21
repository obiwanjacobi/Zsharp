using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTemplateParameter : AstNode,
        IAstIdentifierSite, IAstSymbolSite
    {
        protected AstTemplateParameter()
            : base(AstNodeKind.TemplateParameter)
        { }

        protected AstTemplateParameter(ParserRuleContext context)
            : base(AstNodeKind.TemplateParameter)
        {
            Context = context;
        }

        protected AstTemplateParameter(AstTemplateParameter parameterToCopy)
            : base(AstNodeKind.TemplateParameter)
        {
            Context = parameterToCopy.Context;
            if (parameterToCopy.HasSymbol)
                _symbol = parameterToCopy.Symbol;
            if (parameterToCopy.HasIdentifier)
                this.SetIdentifier(parameterToCopy.Identifier);
        }

        public ParserRuleContext? Context { get; }

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");

        public virtual bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.TemplateParameter, "Identifier must be of kind TemplateParameter");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public bool HasSymbol => _symbol is not null;

        private AstSymbol? _symbol;
        public AstSymbol Symbol => _symbol ?? throw new InternalErrorException("Symbol was not set.");

        public bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);
    }
}
