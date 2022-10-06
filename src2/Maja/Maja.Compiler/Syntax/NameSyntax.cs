namespace Maja.Compiler.Syntax;

public record NameSyntax: SyntaxNode
{
    public NameSyntax(string name)
        => Value = name ?? throw new System.ArgumentNullException(nameof(name));

    public string Value { get; }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnName(this);
}

public sealed record QualifiedNameSyntax : NameSyntax
{
    public QualifiedNameSyntax(string name)
        : base(name)
    { }
}