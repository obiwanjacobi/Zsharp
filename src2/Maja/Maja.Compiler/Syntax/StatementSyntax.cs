namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for statements.
/// </summary>
public abstract record StatementSyntax : SyntaxNode
{
    protected StatementSyntax(string text)
        : base(text)
    { }
}
