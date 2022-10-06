namespace Maja.Compiler.Syntax;

public record NameSyntax: SyntaxNode
{
    public NameSyntax(string name)
        => Value = name ?? throw new System.ArgumentNullException(nameof(name));

    public string Value { get; }
}

public sealed record QualifiedNameSyntax : NameSyntax
{
    public QualifiedNameSyntax(string name)
        : base(name)
    { }
}