using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstTemplateInstanceType : AstTypeDefinition,
        IAstTemplateInstance
    {
        public AstTemplateInstanceType(AstTypeDefinitionIntrinsic intrinsicTypeDef)
        {
            TypeDefinition = intrinsicTypeDef;
        }

        public AstTypeDefinitionIntrinsic TypeDefinition { get; }

        private AstTemplateArgumentMap? _templateArguments;
        public AstTemplateArgumentMap TemplateArguments
            => _templateArguments ?? AstTemplateArgumentMap.Empty;

        public void Instantiate(AstTypeReferenceType type)
        {
            Context = type.Context;
            this.SetIdentifier(type.Identifier!.MakeCopy());

            _templateArguments = new AstTemplateArgumentMap(
                TypeDefinition.TemplateParameters, type.TemplateParameters);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceType(this);
    }
}
