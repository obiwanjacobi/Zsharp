namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for all expression nodes types.
/// </summary>
public abstract class ExpressionSyntax : SyntaxNode
{
    public ExpressionSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// Indicates that this expression whas enclosed in parenthesis.
    /// </summary>
    public bool Precedence { get; internal set; }
}
