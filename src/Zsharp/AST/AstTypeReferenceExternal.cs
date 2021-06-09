using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstTypeReferenceExternal : AstTypeReferenceType
    {
        private readonly TypeReference? _typeReference;

        protected AstTypeReferenceExternal(AstTypeReferenceType typeOrigin)
            : base(typeOrigin)
        { }

        public AstTypeReferenceExternal(TypeReference typeReference)
        {
            _typeReference = typeReference;
            _externalName = new AstExternalName(typeReference.Namespace, typeReference.Name);
        }

        public new AstTypeReferenceExternal? TypeOrigin
            => (AstTypeReferenceExternal?)base.TypeOrigin;

        private readonly AstExternalName? _externalName;
        public AstExternalName ExternalName
            => TypeOrigin?.ExternalName ?? _externalName!;

        public override bool IsExternal => true;

        public override AstTypeReferenceExternal MakeProxy()
        {
            return (TypeOrigin is not null)
                ? new AstTypeReferenceExternal(TypeOrigin)
                : new AstTypeReferenceExternal(this);
        }
    }
}
