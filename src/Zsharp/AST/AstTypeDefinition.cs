namespace Zsharp.AST
{
    public abstract class AstTypeDefinition : AstType
    {
        protected AstTypeDefinition(AstNodeType nodeType = AstNodeType.Type)
            : base(nodeType)
        { }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public virtual bool IsStruct => false;
    }
}