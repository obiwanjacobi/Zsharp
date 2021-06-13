using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeFieldReference : AstTypeField
    {
        protected AstTypeFieldReference(ParserRuleContext context)
            : base(context)
        { }

        public AstTypeFieldDefinition? FieldDefinition
            => Symbol?.DefinitionAs<AstTypeFieldDefinition>();

        public bool TryResolveSymbol()
        {
            this.ThrowIfSymbolEntryNotSet();
            var symbol = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (symbol is not null)
            {
                Symbol = symbol;
                return true;
            }
            return false;
        }
    }
}