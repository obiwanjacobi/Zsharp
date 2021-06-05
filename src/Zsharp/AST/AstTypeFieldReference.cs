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
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry is not null)
            {
                Symbol = entry;
                return true;
            }
            return false;
        }
    }
}