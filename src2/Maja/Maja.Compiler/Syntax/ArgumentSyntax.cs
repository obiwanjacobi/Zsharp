namespace Maja.Compiler.Syntax;

public sealed record ArgumentSyntax : SyntaxNode
{
    public ArgumentSyntax(string text)
        : base(text)
    { }

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnArgument(this);
}