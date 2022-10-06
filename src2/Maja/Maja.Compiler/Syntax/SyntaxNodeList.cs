using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public class SyntaxNodeList : ReadOnlyCollection<SyntaxNode>
{
    private SyntaxNodeList()
        : base(new List<SyntaxNode>())
    { }

    private SyntaxNodeList(IList<SyntaxNode> nodeList)
        : base(nodeList)
    { }

    internal static SyntaxNodeList New(IList<SyntaxNode>? nodeList = null)
        => nodeList is not null
            ? new SyntaxNodeList(nodeList)
            : new SyntaxNodeList();

    private SyntaxNode? _parent;
    public SyntaxNode Parent
    {
        get
        {
            Debug.Assert(_parent is not null, "This is (part of) a root SyntaxNode or the Parent was not set.");
            return _parent!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxNodeList.Parent with null.");
            _parent = value;

            foreach (var node in this.Items)
            {
                node.Parent = _parent;
            }
        }
    }
}