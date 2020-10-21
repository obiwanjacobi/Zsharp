namespace Zsharp.AST
{

    public abstract class AstNode : IAstVisitable
    {
        protected AstNode(AstNodeType nodeType)
        {
            NodeType = nodeType;
        }

        public AstNodeType NodeType { get; }

        private AstNode? _parent;
        public AstNode? Parent => _parent;

        public bool SetParent(AstNode? parent) => Ast.SafeSet(ref _parent, parent);

        public T? GetParent<T>() where T : class
            => _parent as T;

        public T? GetParentRecursive<T>()
            where T : class
        {
            var parent = _parent;
            if (parent != null)
            {
                var typedParent = GetParent<T>();
                if (typedParent != null)
                    return typedParent;

                return parent.GetParentRecursive<T>();
            }

            return null;
        }

        public abstract void Accept(AstVisitor visitor);

        public virtual void VisitChildren(AstVisitor visitor)
        {
            // no-op
        }
    }
}