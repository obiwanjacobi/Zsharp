using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTemplateInstanceStruct : AstTypeDefinitionWithFields
    {
        public AstTemplateInstanceStruct(AstTypeDefinitionStruct templateDefinition)
            : base(AstNodeKind.Struct)
        {
            TemplateDefinition = templateDefinition;
        }

        public AstTypeDefinitionStruct TemplateDefinition { get; }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceStruct(this);

        public void Instantiate(AstTypeReferenceType type)
        {
            Ast.Guard(type.TemplateParameters.Count() == TemplateDefinition.TemplateParameters.Count(),
                $"The number of template parameters do not match the TemplateDefinition {TemplateDefinition.Identifier!.Name}");

            Context = type.Context;
            this.SetIdentifier(new AstIdentifier(type.Identifier!.Name, type.Identifier.IdentifierKind));

            foreach (var field in TemplateDefinition.Fields)
            {
                var fieldDef = new AstTypeDefinitionStructField();
                fieldDef.SetIdentifier(new AstIdentifier(field.Identifier!.Name, field.Identifier.IdentifierKind));
                fieldDef.SetTypeReference(field.TypeReference!.MakeCopy());
                this.AddField(fieldDef);

                type.Symbol!.SymbolTable.Add(fieldDef);
            }
        }
    }
}
