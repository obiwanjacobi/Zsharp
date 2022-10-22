using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A list of syntax nodes or tokens.
/// </summary>
public sealed class SyntaxNodeOrTokenList : ReadOnlyCollection<SyntaxNodeOrToken>
{
    private static readonly List<SyntaxNodeOrToken> Empty = new();

    private SyntaxNodeOrTokenList()
        : base(Empty)
    { }

    private SyntaxNodeOrTokenList(IEnumerable<SyntaxNodeOrToken> tokenList)
        : base(tokenList.ToList())
    { }

    /// <summary>
    /// Creates a new instance for the specified nodeOrTokenList.
    /// </summary>
    /// <param name="nodeOrTokenList">Can be null.</param>
    /// <returns>Never returns null.</returns>
    internal static SyntaxNodeOrTokenList New(IList<SyntaxNodeOrToken>? nodeOrTokenList)
        => nodeOrTokenList is not null && nodeOrTokenList.Count > 0
            ? new SyntaxNodeOrTokenList(nodeOrTokenList)
            : new SyntaxNodeOrTokenList();
}