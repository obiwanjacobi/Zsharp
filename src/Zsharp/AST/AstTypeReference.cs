using Antlr4.Runtime;
using System.Diagnostics;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Identifier}")]
    public abstract class AstTypeReference : AstType
    {
        protected AstTypeReference(ParserRuleContext? context = null)
            : base(AstNodeKind.Type)
        {
            Context = context;
        }

        protected AstTypeReference(AstTypeReference typeToCopy)
            : base(AstNodeKind.Type)
        {
            Context = typeToCopy.Context;
            this.SetIdentifier(typeToCopy.Identifier!.MakeCopy());
            this.Identifier!.SymbolName.TemplatePostfix = string.Empty;
            TrySetSymbol(typeToCopy.Symbol);
            IsInferred = typeToCopy.IsInferred;
        }

        public AstTypeDefinition? TypeDefinition
            => Symbol?.DefinitionAs<AstTypeDefinition>();

        public virtual bool TryResolveSymbol()
        {
            this.ThrowIfSymbolEntryNotSet();
            return Symbol!.SymbolTable.TryResolveDefinition(Symbol);
        }

        public virtual bool IsExternal => false;
        public bool IsInferred { get; set; }

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

        public abstract AstTypeReference MakeCopy();
    }
}