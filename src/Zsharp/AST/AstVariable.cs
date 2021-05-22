using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstVariable : AstNode,
        IAstIdentifierSite, IAstSymbolEntrySite, IAstTypeReferenceSite
    {
        protected AstVariable(AstTypeReference? typeReference = null)
            : base(AstNodeType.Variable)
        {
            TrySetTypeReference(typeReference);
        }

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public bool TryResolve()
        {
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

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