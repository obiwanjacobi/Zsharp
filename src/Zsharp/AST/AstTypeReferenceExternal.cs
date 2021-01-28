using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstTypeReferenceExternal : AstTypeReference
    {
        private readonly TypeReference _typeReference;

        public AstTypeReferenceExternal(TypeReference typeReference)
        {
            _typeReference = typeReference;

            IsOptional = typeReference.IsByReference;
        }
    }
}
