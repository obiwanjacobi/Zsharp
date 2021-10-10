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

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public virtual bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.Variable);

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

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        public override void VisitChildren(AstVisitor visitor)
        {
            TypeReference?.Accept(visitor);
        }
    }
}