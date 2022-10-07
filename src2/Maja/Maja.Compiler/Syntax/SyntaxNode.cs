using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public abstract partial record SyntaxNode : ISyntaxVisitable
{
    // leading/trailing tokens

    protected SyntaxNode(string text)
        => Text = text;

    public string Text { get; }

    private SyntaxNode? _parent;
    public SyntaxNode? Parent
    {
        get { return _parent!; }
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

    public override string ToString()
        => $"{GetType().Name}: {Location}";

    public abstract R Accept<R>(ISyntaxVisitor<R> visitor);
}

