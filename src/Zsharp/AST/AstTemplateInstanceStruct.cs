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

        public AstTypeDefinitionStruct? TemplateDefinition { get; }

        public new IEnumerable<AstTypeDefinitionStructField> Fields
            => base.Fields.Cast<AstTypeDefinitionStructField>();

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceStruct(this);
    }
}
