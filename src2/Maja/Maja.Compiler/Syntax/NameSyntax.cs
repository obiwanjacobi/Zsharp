namespace Maja.Compiler.Syntax;

public record NameSyntax : SyntaxNode
{
    public NameSyntax(string name)
        : base(name)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnName(this);
}

public sealed record QualifiedNameSyntax : NameSyntax
{
    public QualifiedNameSyntax(string name)
        : base(name)
    { }
}