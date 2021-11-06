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
            _externalName = AstName.FromExternal(typeReference.Namespace, typeReference.Name);
        }

        private readonly AstName? _externalName;
        public AstName ExternalName => _externalName!;

        public override bool IsExternal => true;

        public override AstTypeReferenceExternal MakeCopy()
        {
            var typeRef = new AstTypeReferenceExternal(this);
            if (HasSymbol)
                Symbol.AddNode(typeRef);
            return typeRef;
        }
    }
}
