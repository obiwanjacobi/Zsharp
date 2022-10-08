namespace Maja.Compiler.Syntax;

internal record struct SyntaxNodeOrToken
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
}
