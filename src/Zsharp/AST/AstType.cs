using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstType : AstNode,
        IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstType(AstNodeType nodeType)
            : base(nodeType)
        { }

        protected AstType(AstIdentifier identifier)
            : base(AstNodeType.Type)
        {
            TrySetIdentifier(identifier);
        }

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public virtual bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public virtual bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public virtual bool TryResolve()
        {
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        public virtual bool IsEqual(AstType type)
        {
            if (type == null)
                return false;
            if (Identifier == null)
                return false;
            if (type.Identifier == null)
                return false;

            return Identifier.IsEqual(type.Identifier);
        }
    }
}