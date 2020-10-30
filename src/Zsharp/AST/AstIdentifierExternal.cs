namespace Zsharp.AST
{
    public class AstIdentifierExternal : AstIdentifier
    {
        private readonly AstIdentifierType _identifierType;
        private readonly string _name;

        public AstIdentifierExternal(string name, AstIdentifierType identifierType)
        {
            _name = name;
            _identifierType = identifierType;
        }

        public override AstIdentifierType IdentifierType => _identifierType;

        public override string Name => _name;
    }
}
