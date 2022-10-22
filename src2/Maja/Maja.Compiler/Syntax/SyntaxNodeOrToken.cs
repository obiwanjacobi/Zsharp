using System;

namespace Maja.Compiler.Syntax;

/// <summary>
/// A wrapper for either a SyntaxNode or a SyntaxToken.
/// </summary>
public record struct SyntaxNodeOrToken
{
    public SyntaxNodeOrToken(SyntaxNode node)
    {
        Node = node;
        Token = null;
    }

    public SyntaxNodeOrToken(SyntaxToken token)
    {
        Node = null;
        Token = token;
    }

    public SyntaxNode? Node { get; }
    public SyntaxToken? Token { get; }

    public SyntaxNode? Parent
        => Node?.Parent ?? Token?.Parent;

    public string Text
        => Node?.Text ?? Token?.Text ?? String.Empty;

    public bool HasError
        => Node?.HasError ?? Token?.HasError ?? false;
}
