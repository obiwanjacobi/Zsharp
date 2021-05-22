using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTemplateInstanceStruct : AstTypeDefinition
    {
        public AstTemplateInstanceStruct(AstTypeDefinitionStruct templateDefinition)
            : base(AstNodeType.Struct)
        {
            TemplateDefinition = templateDefinition;
        }

        public AstTypeDefinitionStruct TemplateDefinition { get; }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceStruct(this);

        public void Instantiate(AstTypeReference type)
        {
            Ast.Guard(type.TemplateParameters.Count() == TemplateDefinition.TemplateParameters.Count(),
                $"The number of template parameters do not match the TemplateDefinition {TemplateDefinition.Identifier!.Name}");

            this.SetIdentifier(new AstIdentifier(type.Identifier!.Name, type.Identifier.IdentifierType));

            foreach (var field in TemplateDefinition.Fields)
            {
                var fieldDef = new AstTypeDefinitionStructField();
                fieldDef.SetIdentifier(new AstIdentifier(field.Identifier!.Name, field.Identifier.IdentifierType));
                fieldDef.SetTypeReference(field.TypeReference!.MakeProxy());
                AddField(fieldDef);

                type.Symbol!.SymbolTable.Add(fieldDef);
            }
        }
    }
}
