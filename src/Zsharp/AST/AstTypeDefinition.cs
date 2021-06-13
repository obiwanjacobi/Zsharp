namespace Zsharp.AST
{
    public abstract class AstTypeDefinition : AstType
    {
        protected AstTypeDefinition(AstNodeKind nodeKind = AstNodeKind.Type)
            : base(nodeKind)
        { }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public virtual bool IsStruct => false;
    }
}