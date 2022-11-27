using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed class StatementAssignmentSyntax : StatementSyntax
{
    public StatementAssignmentSyntax(string text)
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
}
