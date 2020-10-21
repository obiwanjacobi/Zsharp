namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
        {
            Name = name;
            IdentifierType = identifierType;
        }

        public override string Name { get; }
        public override AstIdentifierType IdentifierType { get; }

        public override AstIdentifier Clone() => new AstIdentifierIntrinsic(Name, IdentifierType);
    }
}