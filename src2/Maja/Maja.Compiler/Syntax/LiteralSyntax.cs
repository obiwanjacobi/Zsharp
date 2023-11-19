namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a literal numerical value.
/// </summary>
public sealed class LiteralNumberSyntax : ExpressionSyntax, ICreateSyntaxNode<LiteralNumberSyntax>
{
    private LiteralNumberSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralNumber;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralNumber(this);
    
    public static LiteralNumberSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represents a literal string value.
/// </summary>
public sealed class LiteralStringSyntax : ExpressionSyntax, ICreateSyntaxNode<LiteralStringSyntax>
{
    private LiteralStringSyntax(string text)
        : base(text.Trim('"'))
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.LiteralString;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnLiteralString(this);
    
    public static LiteralStringSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}
