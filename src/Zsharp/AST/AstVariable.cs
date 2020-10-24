namespace Zsharp.AST
{
    public abstract class AstVariable : AstCodeBlockItem,
        IAstIdentifierSite, IAstSymbolEntrySite, IAstTypeReferenceSite
    {
        protected AstVariable(AstTypeReference? typeReference = null)
            : base(AstNodeType.Variable)
        {
            _typeRef = typeReference;
        }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier) => this.SafeSetParent(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
            => ((IAstIdentifierSite)this).SetIdentifier(identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
            => ((IAstSymbolEntrySite)this).SetSymbol(symbolEntry);

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => this.SafeSetParent(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
            => ((IAstTypeReferenceSite)this).SetTypeReference(typeReference);
    }
}