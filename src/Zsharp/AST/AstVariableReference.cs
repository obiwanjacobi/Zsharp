using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        public AstVariableReference(Variable_refContext context)
        {
            Context = context;
        }

        public AstVariableReference(Variable_assign_structContext context)
        {
            Context = context;
        }

        public AstVariableReference(Variable_assign_valueContext context)
        {
            Context = context;
        }

        internal AstVariableReference(ParserRuleContext context)
        {
            Context = context;
        }

        public bool HasDefinition
            => VariableDefinition != null || ParameterDefinition != null;

        public AstVariableDefinition? VariableDefinition
            => Symbol?.DefinitionAs<AstVariableDefinition>();

        public AstFunctionParameterDefinition? ParameterDefinition
            => Symbol?.DefinitionAs<AstFunctionParameterDefinition>();

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

        private AstTypeFieldReference? _fieldRef;
        public AstTypeFieldReference? Field => _fieldRef;

        public T? FieldAs<T>() where T : class
            => _fieldRef as T;

        public bool TrySetTypeFieldReference(AstTypeFieldReference fieldReference)
            => this.SafeSetParent(ref _fieldRef, fieldReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitVariableReference(this);
    }
}