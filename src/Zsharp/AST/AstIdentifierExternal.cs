namespace Zsharp.AST
{
    public class AstIdentifierExternal : AstIdentifier
    {
        public AstIdentifierExternal(string name, AstIdentifierType identifierType)
            : this(name, name, identifierType)
        { }

        public AstIdentifierExternal(string name, string nativeName, AstIdentifierType identifierType)
            : base(name, identifierType)
        {
            NativeName = nativeName;
        }

        public string NativeName { get; }
    }
}
