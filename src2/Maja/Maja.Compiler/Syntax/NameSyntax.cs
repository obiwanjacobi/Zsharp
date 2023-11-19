namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a name or identifier.
/// </summary>
public class NameSyntax : SyntaxNode, ICreateSyntaxNode<NameSyntax>
{
    private NameSyntax(string name)
        : base(name)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.NameIdentifier;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnName(this);
    
    public static NameSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represents a dot name or identifier.
/// </summary>
public sealed class QualifiedNameSyntax : SyntaxNode, ICreateSyntaxNode<QualifiedNameSyntax>
{
    private QualifiedNameSyntax(string name)
        : base(name)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.QualifiedNameIdentifier;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnQualifiedName(this);

    public static QualifiedNameSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}