using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeField : AstNode,
        IAstIdentifierSite, IAstSymbolSite
    {
        protected AstTypeField(AstNodeKind nodeKind = AstNodeKind.Field)
            : base(nodeKind)
        { }

        protected AstTypeField(ParserRuleContext context, AstNodeKind nodeKind = AstNodeKind.Field)
            : base(nodeKind)
        {
            Context = context;
        }

        public ParserRuleContext? Context { get; }

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");


        public virtual bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.Field);

        protected bool TrySetIdentifier(AstIdentifier identifier, AstIdentifierKind matchKind)
        {
            Ast.Guard(identifier.IdentifierKind == matchKind, $"Identifier must be of kind {matchKind}");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public bool HasSymbol => _symbol is not null;

        private AstSymbol? _symbol;
        public AstSymbol Symbol
        {
            get { return _symbol ?? throw new InternalErrorException("Symbol was not set."); }
            protected set { _symbol = value; }
        }

        public bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);
    }
}
