namespace Zsharp.AST
{
    public enum AstModuleLocality
    {
        Public,
        External
    }

    public abstract class AstModule : AstNode
    {
        protected AstModule(string modName, AstModuleLocality locality)
            : base(AstNodeType.Module)
        {
            Name = modName;
            Locality = locality;
        }

        public AstModuleLocality Locality { get; private set; }

        public string Name { get; private set; }
    }
}