using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstFunction : AstNode,
        IAstCodeBlockLine, IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstFunction()
            : base(AstNodeKind.Function)
        { }

        public ParserRuleContext? Context { get; protected set; }

        public uint Indent { get; set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbol? _symbol;
        public AstSymbol? Symbol
        {
            get { return _symbol; }
            protected set { _symbol = value; }
        }

        public virtual bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public abstract void CreateSymbols(AstSymbolTable functionSymbols, AstSymbolTable? parentSymbols = null);
    }
}