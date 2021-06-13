using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstCurrentContext
    {
        private readonly Stack<AstNode> _current = new();
        protected IEnumerable<AstNode> Nodes => _current;

        public T? TryGetCurrent<T>()
            where T : class
        {
            foreach (var c in _current)
            {
                if (c is T t)
                {
                    return t;
                }
            }
            return null;
        }

        public T GetCurrent<T>()
            where T : class
        {
            T? t = TryGetCurrent<T>();
            Ast.Guard(t, $"GetCurrent() could not find {typeof(T).Name}");
            return t!;
        }

        public void SetCurrent<T>(T current) where T : AstNode
            => _current.Push(current);

        public void RevertCurrent() => _current.Pop();
    }
}
