namespace Zlang.NET.AST
{
    public enum AstNodeType
    {
        None,
        Module,         // root
        File,           // Module.
        Global,         // File.1
        Function,       // File.
        Struct,         // File.
        Enum,           // File.
        Type,           // File., Variable:[1], Parameter:[1]

        CodeBlock,      // File.
        Assignment,     // CodeBlock.
        Branch,         // CodeBlock.
        Expression,     // CodeBlock., Branch.[1]
        Operand,        // Expression.1|2
        Numeric,        // Expression.1

        Identifier,     // Function.1, Struct.1, Enum.1, Assignment.1

        Variable,
        FunctionParameter,
    }

    public abstract class AstNode : IAstVisitable
    {
        protected AstNode(AstNodeType nodeType)
        {
            NodeType = nodeType;
        }

        public AstNodeType NodeType { get; }

        private AstNode? _parent;
        public AstNode? Parent
        {
            get { return _parent; }
        }

        public bool SetParent(AstNode? parent)
        {
            return Ast.SafeSet(ref _parent, parent);
        }

        public T? GetParent<T>()
            where T : class
        {
            return _parent as T;
        }

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