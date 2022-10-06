using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public abstract partial record SyntaxNode
{
    private SyntaxNode? _parent;
    public SyntaxNode Parent
    {
        get
        {
            Debug.Assert(_parent is not null, "This is a root SyntaxNode or the Parent was not set.");
            return _parent!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxNode.Parent with null.");
            _parent = value;
        }
    }

    private SyntaxNodeList? _nodeList;
    public SyntaxNodeList Children
    {
        get
        {
            Debug.Assert(_nodeList is not null, "SyntaxNode is uninitialized. Children are not set.");
            return _nodeList!;
        }
        init
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.Children list with null.");
            _nodeList = value;
            _nodeList.Parent = this;
        }
    }

    public SyntaxLocation Location { get; init; }
}

