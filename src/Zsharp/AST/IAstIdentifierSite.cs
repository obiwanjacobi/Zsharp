namespace Zsharp.AST
{
    public interface IAstIdentifierSite
    {
        bool HasIdentifier { get; }
        AstIdentifier Identifier { get; }
        bool TrySetIdentifier(AstIdentifier identifier);
    }

    public static class AstIdentifierSiteExtensions
    {
        public static void SetIdentifier(this IAstIdentifierSite identifierSite, AstIdentifier identifier)
        {
            if (!identifierSite.TrySetIdentifier(identifier))
                throw new InternalErrorException(
                    "Identifier is already set or null.");
        }
    }
}
