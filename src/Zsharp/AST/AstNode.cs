namespace Zsharp.AST
{

    public abstract class AstNode : IAstVisitable
    {
        protected AstNode(AstNodeKind nodeKind)
        {
            NodeKind = nodeKind;
        }

        public AstNodeKind NodeKind { get; }

        private AstNode? _parent;
        public AstNode? Parent => _parent;

        public bool TrySetParent(AstNode? parent)
            => Ast.SafeSet(ref _parent, parent);

        public void SetParent(AstNode parent)
        {
            if (!TrySetParent(parent))
                throw new InternalErrorException(
                    "Parent Node is already set or null.");
        }

        public T? ParentAs<T>() where T : class
            => _parent as T;

        public T? ParentRecursiveAs<T>()
            where T : class
        {
            var parent = _parent;
            if (parent is not null)
            {
                var typedParent = ParentAs<T>();
                if (typedParent is not null)
                    return typedParent;

                return parent.ParentRecursiveAs<T>();
            }

            return null;
        }

        // detaches from Parent
        public void Orphan()
        {
            _parent = null;
        }

        public abstract void Accept(AstVisitor visitor);

        public virtual void VisitChildren(AstVisitor visitor)
        {
            // no-op
        }
    }
}