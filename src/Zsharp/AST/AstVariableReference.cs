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

        public override bool TrySetIdentifier(AstIdentifier identifier)
        {
            if (identifier.IsIntrinsic)
                // self (parameter) is the only intrinsic that can be referenced in variable.
                return TrySetIdentifier(identifier, AstIdentifierKind.Parameter);

            return base.TrySetIdentifier(identifier);
        }

        public bool HasDefinition
            => VariableDefinition is not null || ParameterDefinition is not null;

        public AstVariableDefinition? VariableDefinition
            => Symbol?.DefinitionAs<AstVariableDefinition>();

        public AstFunctionParameterDefinition? ParameterDefinition
            => Symbol?.DefinitionAs<AstFunctionParameterDefinition>();

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