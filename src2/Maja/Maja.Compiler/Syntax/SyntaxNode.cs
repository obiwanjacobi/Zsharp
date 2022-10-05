using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public abstract partial record SyntaxNode
{
    public SyntaxNode? Parent { get; internal set; }

    private SyntaxNodeList? _nodeList;
    public SyntaxNodeList Children
    {
        get
        {
            Debug.Assert(_nodeList is not null);
            return _nodeList!;
        }
        init
        {
            _nodeList = value;
            _nodeList.Parent = this;
        }
    }

    public SyntaxLocation Location { get; init; }
}

