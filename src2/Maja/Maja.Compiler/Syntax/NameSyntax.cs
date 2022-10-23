namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a name or identifier.
/// </summary>
public record NameSyntax : SyntaxNode
{
    public NameSyntax(string name)
        : base(name)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.NameIdentifier;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnName(this);
}

/// <summary>
/// Represents a dot name or identifier.
/// </summary>
public sealed record QualifiedNameSyntax : NameSyntax
{
    public QualifiedNameSyntax(string name)
        : base(name)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.QualifiedNameIdentifier;
}