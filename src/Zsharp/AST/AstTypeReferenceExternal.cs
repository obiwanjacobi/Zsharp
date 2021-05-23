using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstTypeReferenceExternal : AstTypeReference
    {
        private readonly TypeReference? _typeReference;

        protected AstTypeReferenceExternal(AstTypeReference typeOrigin)
            : base(typeOrigin)
        { }

        public AstTypeReferenceExternal(TypeReference typeReference)
        {
            _typeReference = typeReference;
            _externalName = new AstExternalName(typeReference.Namespace, typeReference.Name);
            IsOptional = typeReference.IsByReference;
        }

        public new AstTypeReferenceExternal? TypeOrigin
            => (AstTypeReferenceExternal?)base.TypeOrigin;

        private readonly AstExternalName? _externalName;
        public AstExternalName ExternalName
            => TypeOrigin?.ExternalName ?? _externalName!;

        public override bool IsExternal => true;

        public override AstTypeReferenceExternal MakeProxy()
        {
            AstTypeReferenceExternal typeRef;

            if (TypeOrigin != null)
                typeRef = new AstTypeReferenceExternal(TypeOrigin);
            else
                typeRef = new AstTypeReferenceExternal(this);

            typeRef.IsTemplateParameter = IsTemplateParameter;

            return typeRef;
        }
    }
}
