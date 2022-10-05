using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public class SyntaxNodeList : ReadOnlyCollection<SyntaxNode>
{
    internal SyntaxNodeList(IList<SyntaxNode> nodeList)
        : base(nodeList)
    { }

    private SyntaxNode? _parent;
    public SyntaxNode Parent
    {
        get
        {
            Debug.Assert(_parent is not null);
            return _parent!;
        }
        internal set
        {
            Debug.Assert(value is not null);
            _parent = value;

            foreach (var node in this.Items)
            {
                node.Parent = _parent;
            }
        }
    }
}