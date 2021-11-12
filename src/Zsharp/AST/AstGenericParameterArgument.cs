using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstGenericParameterArgument : AstGenericParameter,
        IAstTypeReferenceSite
    {
        public AstGenericParameterArgument(AstTypeReference typeReference)
        {
            this.SetTypeReference(typeReference);
        }

        public AstGenericParameterArgument(AstGenericParameterArgument argumentToCopy)
            : base(argumentToCopy)
        {
            this.SetTypeReference(argumentToCopy.TypeReference.MakeCopy());
        }

        internal AstGenericParameterArgument(ParserRuleContext context)
            : base(context)
        { }

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitGenericParameterReference(this);
    }
}
