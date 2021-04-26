using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstTypeReferenceExternal : AstTypeReference
    {
        private readonly TypeReference _typeReference;

        public AstTypeReferenceExternal(TypeReference typeReference)
        {
            _typeReference = typeReference;
            ExternalName = new AstExternalName(typeReference.Namespace, typeReference.Name);
            IsOptional = typeReference.IsByReference;
        }

        public AstExternalName ExternalName { get; }
    }
}
