using Antlr4.Runtime;
using System.Diagnostics;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public abstract class AstTypeReference : AstType
    {
        protected AstTypeReference(ParserRuleContext? context = null)
            : base(AstNodeType.Type)
        {
            Context = context;
        }

        protected AstTypeReference(AstTypeReference typeOrigin)
            : base(AstNodeType.Type)
        {
            Context = typeOrigin.Context;
            this.SetIdentifier(typeOrigin.Identifier!);
            TrySetSymbol(typeOrigin.Symbol!);
            _typeOrigin = typeOrigin;
        }

        public AstTypeDefinition? TypeDefinition
            => Symbol?.DefinitionAs<AstTypeDefinition>();

        public virtual bool TryResolveSymbol()
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

        public virtual bool IsExternal => false;

        public override bool IsEqual(AstType type)
        {
            if (!base.IsEqual(type))
                return false;

            if (type is not AstTypeReference typedThat)
                return false;

            var typeDef = TypeDefinition;
            var thatTypeDef = typedThat.TypeDefinition;

            if (typeDef is not null &&
                thatTypeDef is not null)
            {
                return typeDef.IsEqual(thatTypeDef);
            }
            return false;
        }

        private readonly AstTypeReference? _typeOrigin;
        // points to the origin of this 'proxy'.
        public AstTypeReference? TypeOrigin => _typeOrigin;

        public bool IsProxy => _typeOrigin is not null;

        public abstract AstTypeReference MakeProxy();
    }
}