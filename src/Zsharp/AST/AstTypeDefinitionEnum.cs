using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnum : AstTypeDefinitionWithFields,
        IAstCodeBlockLine, IAstSymbolTableSite
    {
        internal AstTypeDefinitionEnum(ParserRuleContext context, AstSymbolTable parentTable)
            : base(AstNodeKind.Enum)
        {
            Context = context;
            SymbolTable = new AstSymbolTable("", parentTable);
        }

        public uint Indent { get; set; }

        public AstSymbolTable SymbolTable { get; }

        public override bool TrySetIdentifier(AstIdentifier identifier)
        {
            var success = base.TrySetIdentifier(identifier);

            if (success)
                SymbolTable.SetName(identifier.SymbolName.CanonicalName.FullName);

            return success;
        }

        public new IEnumerable<AstTypeDefinitionEnumOption> Fields
            => base.Fields.Cast<AstTypeDefinitionEnumOption>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionEnum(this);

        public override bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
            => throw new InternalErrorException("Cannot add Template Parameters to an Enum.");
    }
}
