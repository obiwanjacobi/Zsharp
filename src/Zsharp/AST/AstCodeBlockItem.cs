namespace Zsharp.AST
{
    public abstract class AstCodeBlockItem : AstNode
    {
        protected AstCodeBlockItem(AstNodeType nodeType)
            : base(nodeType)
        { }

        public int Indent { get; set; }
    }
}
