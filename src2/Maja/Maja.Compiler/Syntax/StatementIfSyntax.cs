using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents the 'if' statement.
/// </summary>
public class StatementIfSyntax : StatementSyntax, ICreateSyntaxNode<StatementIfSyntax>
{
    private StatementIfSyntax(string text)
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
    
    public static StatementIfSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

public abstract class StatementElseClauseSyntax : SyntaxNode
{
    protected StatementElseClauseSyntax(string text)
        : base(text)
    { }

    /// <summary>
    /// The code that is executed when the if-condition executes the else clause.
    /// </summary>
    public CodeBlockSyntax CodeBlock
        => ChildNodes.OfType<CodeBlockSyntax>().Single();
}

/// <summary>
/// Represents an else-if branch for an if statement.
/// </summary>
public sealed class StatementElseIfSyntax : StatementElseClauseSyntax, ICreateSyntaxNode<StatementElseIfSyntax>
{
    private StatementElseIfSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementElseIf;

    /// <summary>
    /// The condition expression of the if statement.
    /// </summary>
    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

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

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElseIf(this);
    
    public static StatementElseIfSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}

/// <summary>
/// Represents an else branch for an if statement.
/// </summary>
public sealed class StatementElseSyntax : StatementElseClauseSyntax, ICreateSyntaxNode<StatementElseSyntax>
{
    private StatementElseSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementElse;

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElse(this);
    
    public static StatementElseSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location, 
            Children = children, 
            ChildNodes = childNodes, 
            TrailingTokens = trailingTokens
        };
}
