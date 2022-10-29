using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents the 'if' statement.
/// </summary>
public class StatementIfSyntax : StatementSyntax
{
    public StatementIfSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementIf;

    /// <summary>
    /// The condition expression of the if statement.
    /// </summary>
    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    /// <summary>
    /// The code executed when the condition expression evaluates to true.
    /// </summary>
    public CodeBlockSyntax CodeBlock
        => ChildNodes.OfType<CodeBlockSyntax>().Single();

    /// <summary>
    /// The 'else' branch, if any.
    /// </summary>
    public StatementElseSyntax? Else
        => ChildNodes.OfType<StatementElseSyntax>().SingleOrDefault();

    /// <summary>
    /// The 'else if' branch if any.
    /// </summary>
    public StatementElseIfSyntax? ElseIf
        => ChildNodes.OfType<StatementElseIfSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementIf(this);
}

/// <summary>
/// Represents an else-if branch for an if statement.
/// </summary>
public sealed class StatementElseIfSyntax : StatementIfSyntax
{
    public StatementElseIfSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementElseIf;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElseIf(this);
}

/// <summary>
/// Represents an else branch for an if statement.
/// </summary>
public sealed class StatementElseSyntax : StatementSyntax
{
    public StatementElseSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementElse;

    /// <summary>
    /// The code that is executed when the if-condition evaluates to false.
    /// </summary>
    public CodeBlockSyntax CodeBlock
        => ChildNodes.OfType<CodeBlockSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElse(this);
}
