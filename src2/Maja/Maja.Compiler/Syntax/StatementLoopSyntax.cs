using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed class StatementLoopSyntax : StatementSyntax, ICreateSyntaxNode<StatementLoopSyntax>
{
    public StatementLoopSyntax(string text)
        : base(text)
    { }

    public ExpressionSyntax? Expression
        => ChildNodes.OfType<ExpressionSyntax>().SingleOrDefault();

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementLoop;

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementLoop(this);

    public static StatementLoopSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
