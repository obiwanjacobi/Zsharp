using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A list of (child) syntax nodes.
/// </summary>
public sealed class SyntaxNodeList : ReadOnlyCollection<SyntaxNode>
{
    private static readonly List<SyntaxNode> Empty = new();

    private SyntaxNodeList()
        : base(Empty)
    { }

    private SyntaxNodeList(IList<SyntaxNode> nodeList)
        : base(nodeList)
    { }

    /// <summary>
    /// Returns a new instance for the specified nodeList.
    /// </summary>
    /// <param name="nodeList">Can be null.</param>
    /// <returns>Never returns null.</returns>
    internal static SyntaxNodeList New(IList<SyntaxNode>? nodeList = null)
        => nodeList is not null && nodeList.Count > 0
            ? new SyntaxNodeList(nodeList)
            : new SyntaxNodeList();

    private SyntaxNode? _parent;
    /// <summary>
    /// The syntax node this list is part of and will tag all nodes added.
    /// </summary>
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