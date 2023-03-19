namespace Maja.Compiler.Syntax;

/// <summary>
/// Common base class for all expression nodes types.
/// </summary>
public abstract class ExpressionSyntax : SyntaxNode
{
    protected ExpressionSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// Indicates that this expression was enclosed in parenthesis.
    /// </summary>
    public bool Precedence { get; internal set; }

    public virtual bool IsPrecedenceValid
        => true;
}
