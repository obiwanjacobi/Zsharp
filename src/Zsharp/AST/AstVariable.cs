namespace Zsharp.AST
{
    public abstract class AstVariable : AstCodeBlockItem, IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstVariable()
            : base(AstNodeType.Variable)
        { }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool SetIdentifier(AstIdentifier identifier) => this.SafeSetParent(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool SetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}