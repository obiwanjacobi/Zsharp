namespace Zsharp.AST
{
    public abstract class AstFunctionParameter : AstNode,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        protected AstFunctionParameter()
            : base(AstNodeKind.FunctionParameter)
        { }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public virtual bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public override void VisitChildren(AstVisitor visitor)
            => TypeReference?.Accept(visitor);

        public void ReplaceTypeReference(AstTypeReference? typeReference)
        {
            if (_typeReference is not null)
                _typeReference.Symbol?.RemoveReference(_typeReference);

            _typeReference = typeReference;
        }
    }
}
