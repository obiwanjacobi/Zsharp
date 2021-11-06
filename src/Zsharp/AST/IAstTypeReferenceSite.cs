namespace Zsharp.AST
{
    public interface IAstTypeReferenceSite
    {
        bool HasTypeReference { get; }
        AstTypeReference TypeReference { get; }
        bool TrySetTypeReference(AstTypeReference? typeReference);
    }

    public static class AstTypeReferenceSiteExtensions
    {
        public static void SetTypeReference(this IAstTypeReferenceSite typeReferenceSite, AstTypeReference typeReference)
        {
            if (!typeReferenceSite.TrySetTypeReference(typeReference))
                throw new InternalErrorException(
                    "TypeReference is already set or null.");
        }
    }
}