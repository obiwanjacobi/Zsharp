using System.Linq;

namespace Maja.Compiler.Syntax;

public record StatementIfSyntax : StatementSyntax
{
    public StatementIfSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax Expression
        => Children.OfType<ExpressionSyntax>().Single();

    public CodeBlockSyntax CodeBlock
        => Children.OfType<CodeBlockSyntax>().Single();

    public StatementElseSyntax? Else
        => Children.OfType<StatementElseSyntax>().SingleOrDefault();

    public StatementElseIfSyntax? ElseIf
        => Children.OfType<StatementElseIfSyntax>().SingleOrDefault();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementIf(this);
}

public sealed record StatementElseIfSyntax : StatementIfSyntax
{
    public StatementElseIfSyntax(string text)
        : base(text)
    { }

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElseIf(this);
}

public sealed record StatementElseSyntax : StatementSyntax
{
    public StatementElseSyntax(string text)
        : base(text)
    { }

    public CodeBlockSyntax CodeBlock
        => Children.OfType<CodeBlockSyntax>().Single();

    public sealed override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementElse(this);
}
