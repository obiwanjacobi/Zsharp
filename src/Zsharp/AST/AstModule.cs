namespace Zsharp.AST
{
    public enum AstModuleLocality
    {
        Public,
        External
    }

    public abstract class AstModule : AstNode,
        IAstIdentifierSite
    {
        protected AstModule(string modName, AstModuleLocality locality)
            : base(AstNodeType.Module)
        {
            Identifier = new AstIdentifier(modName, AstIdentifierType.Module);
            Locality = locality;
        }

        public AstModuleLocality Locality { get; private set; }

        //public string Name { get; private set; }

        public AstIdentifier? Identifier { get; }

        public void SetIdentifier(AstIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }
    }
}