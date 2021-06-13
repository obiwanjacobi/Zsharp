using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeField : AstNode,
        IAstIdentifierSite, IAstSymbolEntrySite
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

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public virtual bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.Field);

        protected bool TrySetIdentifier(AstIdentifier identifier, AstIdentifierKind matchKind)
        {
            Ast.Guard(identifier.IdentifierKind == matchKind, $"Identifier must be of kind {matchKind}");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        private AstSymbol? _symbol;
        public AstSymbol? Symbol
        {
            get { return _symbol; }
            protected set { _symbol = value; }
        }

        public bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}
