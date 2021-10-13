using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstGenericParameterReference : AstGenericParameter,
        IAstTypeReferenceSite
    {
        public AstGenericParameterReference(AstTypeReference typeReference)
        {
            this.SetTypeReference(typeReference);
        }

        public AstGenericParameterReference(AstGenericParameterReference parameterToCopy)
            : base(parameterToCopy)
        {
            this.SetTypeReference(parameterToCopy.TypeReference!.MakeCopy());
        }

        internal AstGenericParameterReference(ParserRuleContext context)
            : base(context)
        { }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitGenericParameterReference(this);
    }
}