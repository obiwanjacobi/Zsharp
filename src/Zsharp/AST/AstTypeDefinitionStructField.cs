using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStructField : AstTypeFieldDefinition,
        IAstTypeReferenceSite
    {
        public AstTypeDefinitionStructField()
        { }

        internal AstTypeDefinitionStructField(ParserRuleContext context)
            : base(context)
        { }

        public bool IsTemplate 
            => _typeReference?.IsTemplateParameter ?? false;

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReferenceType? _typeReference;
        public AstTypeReferenceType TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference was not set.");

        AstTypeReference IAstTypeReferenceSite.TypeReference => TypeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, (AstTypeReferenceType?)typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStructField(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            if (HasTypeReference)
                TypeReference.Accept(visitor);
        }
    }
}
