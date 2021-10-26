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
            _externalName = new AstName(typeReference.Namespace, typeReference.Name, AstNameKind.External);
        }

        private readonly AstName? _externalName;
        public AstName ExternalName => _externalName!;

        public override bool IsExternal => true;

        public override AstTypeReferenceExternal MakeCopy()
        {
            var typeRef = new AstTypeReferenceExternal(this);
            Symbol?.AddNode(typeRef);
            return typeRef;
        }
    }
}
