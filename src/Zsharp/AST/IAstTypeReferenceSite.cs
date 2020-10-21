using System;

namespace Zsharp.AST
{
    public interface IAstTypeReferenceSite
    {
        AstTypeReference? TypeReference { get; }
        bool SetTypeReference(AstTypeReference typeReference);

        public void ThrowIfTypeReferenceNotSet()
            => _ = TypeReference ?? throw new InvalidOperationException("TypeReference is not set.");
    }
}