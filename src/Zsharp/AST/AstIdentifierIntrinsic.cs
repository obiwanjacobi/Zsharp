namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
        {
            Name = name;
            IdentifierType = identifierType;
        }

        public static readonly AstIdentifierIntrinsic Self = new AstIdentifierIntrinsic("self", AstIdentifierType.Parameter);

        public override string Name { get; }
        public override AstIdentifierType IdentifierType { get; }
    }
}