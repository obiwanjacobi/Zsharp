using System.Linq;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Assignment of a variable (as a statement, not an expression)
/// </summary>
public sealed class StatementAssignmentSyntax : StatementSyntax, ICreateSyntaxNode<StatementAssignmentSyntax>
{
    private StatementAssignmentSyntax(string text)
        : base(text)
    { }

    public override SyntaxKind SyntaxKind
        => SyntaxKind.StatementAssignment;

    public NameSyntax Name
        => ChildNodes.OfType<NameSyntax>().Single();

    public ExpressionSyntax Expression
        => ChildNodes.OfType<ExpressionSyntax>().Single();

    public override R Accept<R>(ISyntaxVisitor<R> visitor)
        => visitor.OnStatementAssignment(this);

    public static StatementAssignmentSyntax Create(string text, SyntaxLocation location, SyntaxNodeOrTokenList children, SyntaxNodeList childNodes, SyntaxTokenList trailingTokens)
        => new(text)
        {
            Location = location,
            Children = children,
            ChildNodes = childNodes,
            TrailingTokens = trailingTokens
        };
}
