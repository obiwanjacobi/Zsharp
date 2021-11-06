using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstFunction : AstNode,
        IAstCodeBlockLine, IAstIdentifierSite, IAstSymbolSite
    {
        protected AstFunction()
            : base(AstNodeKind.Function)
        { }

        public ParserRuleContext? Context { get; protected set; }

        public uint Indent { get; set; }

        public bool HasIdentifier => _identifier is not null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier 
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.Function, "Identifier must be of kind Function");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public bool HasSymbol => _symbol is not null;

        private AstSymbol? _symbol;
        public AstSymbol Symbol
        {
            get { return _symbol ?? throw new InternalErrorException("Symbol was not set."); }
            protected set { _symbol = value; }
        }

        public virtual bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);

        public abstract void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null);
    }
}