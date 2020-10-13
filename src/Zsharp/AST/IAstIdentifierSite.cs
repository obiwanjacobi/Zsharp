namespace Zsharp.AST
{
    public interface IAstIdentifierSite
    {
        AstIdentifier? Identifier { get; }
        bool SetIdentifier(AstIdentifier identifier);
    }
}
