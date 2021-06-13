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

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol
        {
            get { return _symbol; }
            protected set { _symbol = value; }
        }

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}
