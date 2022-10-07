using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Maja.Compiler.Syntax;

public class SyntaxTokenList : ReadOnlyCollection<SyntaxToken>
{
    private SyntaxTokenList()
        : base(new List<SyntaxToken>())
    { }

    private SyntaxTokenList(IList<SyntaxToken> tokenList)
        : base(tokenList)
    { }

    internal static SyntaxTokenList New(IList<SyntaxToken>? tokenList = null)
        => tokenList is not null
            ? new SyntaxTokenList(tokenList)
            : new SyntaxTokenList();

    private SyntaxNode? _parent;
    public SyntaxNode Parent
    {
        get
        {
            Debug.Assert(_parent is not null, "This is (part of) a root SyntaxToken or the Parent was not set.");
            return _parent!;
        }
        internal set
        {
            Debug.Assert(value is not null, "Cannot clear a SyntaxTokenList.Parent with null.");
            _parent = value;

            foreach (var token in this.Items)
            {
                token.Parent = _parent;
            }
        }
    }
}