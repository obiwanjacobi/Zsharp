using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstTemplateInstanceStruct : AstTypeDefinitionWithFields,
        IAstTemplateInstance
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
                $"The number of template parameters do not match the TemplateDefinition {TemplateDefinition.Identifier!.NativeFullName}");

            Context = type.Context;
            this.SetIdentifier(type.Identifier!.MakeCopy());

            _templateArguments = new AstTemplateArgumentMap(
                TemplateDefinition.TemplateParameters, type.TemplateParameters);

            foreach (var field in TemplateDefinition.Fields)
            {
                var fieldDef = new AstTypeDefinitionStructField();
                fieldDef.SetIdentifier(field.Identifier!.MakeCopy());
                if (field.IsTemplate)
                { 
                    var templateArgument = _templateArguments.LookupArgument(field.TypeReference!.Identifier!);
                    Ast.Guard(templateArgument, $"No Template Argument was found for {field.TypeReference!.Identifier!.CanonicalFullName}");
                    fieldDef.SetTypeReference(templateArgument!.TypeReference!.MakeCopy());
                }
                else
                    fieldDef.SetTypeReference(field.TypeReference!.MakeCopy());

                this.AddField(fieldDef);
                type.Symbol!.SymbolTable.Add(fieldDef);
            }
        }

        private AstTemplateArgumentMap? _templateArguments;
        public AstTemplateArgumentMap TemplateArguments
            => _templateArguments ?? AstTemplateArgumentMap.Empty;
    }
}
