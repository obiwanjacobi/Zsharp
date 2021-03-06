﻿namespace Zsharp.AST
{
    public abstract class AstFunctionParameter : AstNode,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        protected AstFunctionParameter()
            : base(AstNodeKind.FunctionParameter)
        { }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.Parameter, "Identifier must be of kind Parameter");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        private AstSymbol? _symbol;
        public AstSymbol? Symbol => _symbol;

        public virtual bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public override void VisitChildren(AstVisitor visitor)
            => TypeReference?.Accept(visitor);

        public AstTypeReference? ReplaceTypeReference(AstTypeReference? typeReference)
        {
            var oldTypeRef = _typeReference;
            _typeReference = typeReference;
            return oldTypeRef;
        }
    }
}
