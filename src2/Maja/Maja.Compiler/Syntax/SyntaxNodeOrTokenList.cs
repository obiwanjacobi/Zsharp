using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A list of syntax nodes or tokens.
/// </summary>
public sealed class SyntaxNodeOrTokenList : ReadOnlyCollection<SyntaxNodeOrToken>
{
    private SyntaxNodeOrTokenList()
        : base(ReadOnlyCollection<SyntaxNodeOrToken>.Empty)
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

    public static new SyntaxNodeOrTokenList Empty { get; } = new SyntaxNodeOrTokenList();

    /// <summary>
    /// Returns SyntaxNodes of the type T.
    /// </summary>
    /// <typeparam name="T">A SyntaxNode derived type.</typeparam>
    /// <returns>Never returns null.</returns>
    public IEnumerable<T> NodesOfType<T>() where T : SyntaxNode
        => this.Where(ton => ton.Node is T).Select(ton => (T)ton.Node!);

    /// <summary>
    /// Returns SyntaxTokens of the type T.
    /// </summary>
    /// <typeparam name="T">A SyntaxToken derived type.</typeparam>
    /// <returns>Never returns null.</returns>
    public IEnumerable<T> TokensOfType<T>() where T : SyntaxToken
        => this.Where(ton => ton.Token is T).Select(ton => (T)ton.Token!);
}