using Antlr4.Runtime;
using System;

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

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier) => Ast.SafeSet(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException(
                    "SymbolEntry is already set or null.");
        }

        public bool TryResolve()
        {
            var entry = Symbol?.SymbolTable.Resolve(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => this.SafeSetParent(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }
    }
}