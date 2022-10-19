namespace Maja.Compiler.Syntax;

public abstract record ExpressionSyntax : SyntaxNode
{
    public ExpressionSyntax(string text)
        : base(text)
    { }

    public bool Precedence { get; internal set; }
}
