using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public abstract class AstTemplateParameter : AstNode,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        protected AstTemplateParameter(ParserRuleContext context)
            : base(AstNodeType.TemplateParameter)
        {
            Context = context;
        }

        public ParserRuleContext Context { get; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference typeReference)
        {
            return this.SafeSetParent(ref _typeReference, typeReference);
        }

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException(
                    "Symbol is already set or null.");
        }

        public abstract bool TryResolve();
    }
}