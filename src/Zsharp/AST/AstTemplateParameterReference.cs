using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTemplateParameterReference : AstTemplateParameter,
        IAstTypeReferenceSite
    {
        internal AstTemplateParameterReference(ParserRuleContext context)
            : base(context)
        { }

        public AstTemplateParameterReference(AstTypeReference typeReference)
        {
            this.SetTypeReference(typeReference);
        }

        public AstTemplateParameterReference(AstTemplateParameterReference parameterToCopy)
            : base(parameterToCopy)
        {
            this.SetTypeReference(parameterToCopy.TypeReference.MakeCopy());
        }

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterReference(this);
    }
}