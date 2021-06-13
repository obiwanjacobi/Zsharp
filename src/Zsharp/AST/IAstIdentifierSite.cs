namespace Zsharp.AST
{
    public interface IAstIdentifierSite
    {
        AstIdentifier? Identifier { get; }
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

        public static void ThrowIfIdentifierNotSet(this IAstIdentifierSite identifierSite)
            => _ = identifierSite.Identifier ??
                throw new InternalErrorException("Identifier is not set.");
    }
}
