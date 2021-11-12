using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTemplateParameterArgument : AstTemplateParameter,
        IAstTypeReferenceSite
    {
        internal AstTemplateParameterArgument(ParserRuleContext context)
            : base(context)
        { }

        public AstTemplateParameterArgument(AstTypeReference typeReference)
        {
            this.SetTypeReference(typeReference);
        }

        public AstTemplateParameterArgument(AstTemplateParameterArgument argumentToCopy)
            : base(argumentToCopy)
        {
            this.SetTypeReference(argumentToCopy.TypeReference.MakeCopy());
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
