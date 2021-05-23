using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeFieldReference : AstNode,
        IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstTypeFieldReference(ParserRuleContext context)
            : base(AstNodeType.Field)
        {
            Context = context;
        }

        public ParserRuleContext Context { get; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        public AstTypeFieldDefinition? FieldDefinition
            => Symbol?.DefinitionAs<AstTypeFieldDefinition>();

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
    }
}