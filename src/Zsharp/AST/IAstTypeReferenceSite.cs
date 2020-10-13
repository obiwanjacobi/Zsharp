namespace Zsharp.AST
{
    public interface IAstTypeReferenceSite
    {
        AstTypeReference? TypeReference { get; }
        bool SetTypeReference(AstTypeReference typeReference);
    }
}