namespace Zsharp.AST
{
    public abstract class AstCodeBlockItem : AstNode
    {
        protected AstCodeBlockItem(AstNodeType nodeType)
            : base(nodeType)
        { }

        public int Indent { get; set; }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitCodeBlockItem(this);
        }
    }
}
