using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnum : AstTypeDefinitionWithFields,
        IAstCodeBlockItem, IAstSymbolTableSite
    {
        public AstTypeDefinitionEnum(Enum_defContext context, AstSymbolTable parentTable)
            : base(AstNodeKind.Enum)
        {
            Symbols = new AstSymbolTable("", parentTable);
            Context = context;
        }

        public uint Indent { get; set; }

        public AstSymbolTable Symbols { get; }

        public override bool TrySetIdentifier(AstIdentifier? identifier)
        {
            var success = base.TrySetIdentifier(identifier);

            if (success)
                Symbols.SetName(identifier!.CanonicalName);

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
