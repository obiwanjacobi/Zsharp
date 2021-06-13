using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstType : AstNode,
        IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstType(AstNodeKind nodeKind)
            : base(nodeKind)
        { }

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public virtual bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.Type, "Identifier must be of kind Type");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        private AstSymbol? _symbol;
        public AstSymbol? Symbol
        {
            get { return _symbol; }
            protected set { _symbol = value; }
        }

        public virtual bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public virtual bool IsEqual(AstType type)
        {
            if (type is null)
                return false;
            if (Identifier is null)
                return false;
            if (type.Identifier is null)
                return false;

            return Identifier.IsEqual(type.Identifier);
        }
    }
}