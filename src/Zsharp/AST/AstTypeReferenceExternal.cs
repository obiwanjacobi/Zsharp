using Zsharp.External.Metadata;

namespace Zsharp.AST
{
    public class AstTypeReferenceExternal : AstTypeReferenceType
    {
        private readonly TypeMetadata? _typeReference;

        protected AstTypeReferenceExternal(AstTypeReferenceType typeOrigin)
            : base(typeOrigin)
        { }

        public AstTypeReferenceExternal(TypeMetadata typeReference)
        {
            _typeReference = typeReference;
            _externalName = new AstExternalName(typeReference.Namespace, typeReference.Name);
        }

        private readonly AstExternalName? _externalName;
        public AstExternalName ExternalName => _externalName!;

        public override bool IsExternal => true;

        public override AstTypeReferenceExternal MakeCopy()
        {
            var typeRef = new AstTypeReferenceExternal(this);
            Symbol?.AddNode(typeRef);
            return typeRef;
        }
    }
}
