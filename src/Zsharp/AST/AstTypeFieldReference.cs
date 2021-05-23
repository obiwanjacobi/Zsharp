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

        public bool TryResolve()
        {
            this.ThrowIfSymbolEntryNotSet();
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry != null)
            {
                Symbol = entry;
                return true;
            }
            return false;
        }
    }
}