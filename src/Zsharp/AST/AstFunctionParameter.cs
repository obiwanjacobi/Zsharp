﻿using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstFunctionParameter : AstNode,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolSite
    {
        protected AstFunctionParameter(ParserRuleContext? context)
            : base(AstNodeKind.FunctionParameter)
        {
            Context = context;
        }

        public ParserRuleContext? Context { get; }

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

        public virtual bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);

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
