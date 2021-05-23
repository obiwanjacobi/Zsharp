namespace Zsharp.AST
{
    public abstract class AstFunctionParameter : AstNode,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        protected AstFunctionParameter()
            : base(AstNodeType.FunctionParameter)
        { }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public virtual bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public override void VisitChildren(AstVisitor visitor)
            => TypeReference?.Accept(visitor);
    }
}
