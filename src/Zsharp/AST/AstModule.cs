namespace Zsharp.AST
{
    public enum AstModuleLocality
    {
        Public,
        External
    }

    public class AstModule : AstNode
    {
        public AstModule(string modName, AstModuleLocality locality)
            : base(AstNodeType.Module)
        {
            Name = modName;
            Locality = locality;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitModule(this);
        }

        public AstModuleLocality Locality { get; private set; }

        public string Name { get; private set; }
    }
}