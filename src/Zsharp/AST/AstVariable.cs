using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstVariable : AstNode,
        IAstIdentifierSite, IAstSymbolSite, IAstTypeReferenceSite
    {
        protected AstVariable(AstTypeReference? typeReference = null)
            : base(AstNodeKind.Variable)
        {
            TrySetTypeReference(typeReference);
        }

        public ParserRuleContext? Context { get; protected set; }

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");


        public virtual bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.Variable);

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

        public bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void VisitChildren(AstVisitor visitor)
        {
            if (HasTypeReference)
                TypeReference.Accept(visitor);
        }
    }
}