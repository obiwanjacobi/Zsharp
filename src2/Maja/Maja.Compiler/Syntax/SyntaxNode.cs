using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public abstract partial record SyntaxNode : ISyntaxVisitable
{
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
    
    public SyntaxLocation Location { get; init; }
    
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

    public bool HasLeadingTokens => _leadingTokens is not null;

    private SyntaxTokenList? _leadingTokens;
    public SyntaxTokenList LeadingTokens
    {
        get
        {
            Debug.Assert(_leadingTokens is not null, "SyntaxNode is uninitialized. LeadingTokens are not set.");
            return _leadingTokens!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.LeadingTokens list with null.");
            _leadingTokens = value;
            _leadingTokens.Parent = this;
        }
    }

    public bool HasTrailingTokens => _trailingTokens is not null;

    private SyntaxTokenList? _trailingTokens;
    public SyntaxTokenList TrailingTokens
    {
        get
        {
            Debug.Assert(_trailingTokens is not null, "SyntaxNode is uninitialized. TrailingTokens are not set.");
            return _trailingTokens!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear the SyntaxNode.TrailingTokens list with null.");
            _trailingTokens = value;
            _trailingTokens.Parent = this;
        }
    }

    public override string ToString()
        => $"{GetType().Name}: {Location}";

    public abstract R Accept<R>(ISyntaxVisitor<R> visitor);
}

