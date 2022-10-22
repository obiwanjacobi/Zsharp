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
    {
        get
        {
            return Node?.Parent ?? Token?.Parent;
        }
        internal set
        {
            if (Node is SyntaxNode node)
                node.Parent = value;
            if (Token is SyntaxToken token)
                token.Parent = value!;
        }
    }
}
