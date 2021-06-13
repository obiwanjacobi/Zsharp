using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStruct : AstTypeDefinitionWithFields,
        IAstCodeBlockLine, IAstSymbolTableSite
    {
        public AstTypeDefinitionStruct(Struct_defContext context, AstSymbolTable parentTable)
            : base(AstNodeKind.Struct)
        {
            Symbols = new AstSymbolTable("", parentTable);
            Context = context;
        }

        public uint Indent { get; set; }

        public override bool IsStruct => true;

        public AstSymbolTable Symbols { get; }

        public override bool TrySetIdentifier(AstIdentifier? identifier)
        {
            var success = base.TrySetIdentifier(identifier);

            if (success)
                Symbols.SetName(identifier!.CanonicalName);

            return success;
        }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public bool TryAddField(AstTypeDefinitionStructField field)
        {
            var success = base.TryAddField(field);

            if (success)
                Symbols.Add(field);

            return success;
        }

        public override bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (base.TryAddTemplateParameter(templateParameter))
            {
                Symbols.Add(templateParameter);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStruct(this);
    }
}
