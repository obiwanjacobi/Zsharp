using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A list of syntax tokens.
/// </summary>
public sealed class SyntaxTokenList : ReadOnlyCollection<SyntaxToken>
{
    private SyntaxTokenList()
        : base(ReadOnlyCollection<SyntaxToken>.Empty)
    { }

    private SyntaxTokenList(IEnumerable<SyntaxToken> tokenList)
        : base(tokenList.ToList())
    { }

    /// <summary>
    /// Creates a new instance for the specified tokenList.
    /// </summary>
    /// <param name="tokenList">Can be null.</param>
    /// <returns>Never returns null.</returns>
    internal static SyntaxTokenList New(IList<SyntaxToken>? tokenList)
        => tokenList is not null && tokenList.Count > 0
            ? new SyntaxTokenList(tokenList)
            : new SyntaxTokenList();

    public static new SyntaxTokenList Empty => new SyntaxTokenList();

    private SyntaxNode? _parent;
    /// <summary>
    /// The parent syntax node this list belongs to.
    /// </summary>
    public SyntaxNode Parent
    {
        get
        {
            Debug.Assert(_parent is not null, "This SyntaxTokenList is (part of) a root SyntaxToken or the Parent was not set.");
            return _parent!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxTokenList.Parent with null.");
            Debug.Assert(_parent is null, "SyntaxTokenList.Parent is already set.");
            _parent = value;

            foreach (var token in Items)
            {
                token.Parent = _parent;
            }
        }
    }
}