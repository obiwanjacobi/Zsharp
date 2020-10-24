using System;

namespace Zsharp.AST
{
    public interface IAstTypeReferenceSite
    {
        AstTypeReference? TypeReference { get; }
        bool TrySetTypeReference(AstTypeReference typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

        public void ThrowIfTypeReferenceNotSet()
            => _ = TypeReference ?? throw new InvalidOperationException("TypeReference is not set.");
    }
}