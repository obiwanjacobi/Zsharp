namespace Maja.Compiler.Syntax;

public abstract record StatementSyntax : SyntaxNode
{
    protected StatementSyntax(string text)
        : base(text)
    { }
}
