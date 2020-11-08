namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        public static readonly AstIdentifierIntrinsic Self = new AstIdentifierIntrinsic("self", AstIdentifierType.Parameter);

        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
            : base(name, identifierType)
        { }
    }
}