using System.Linq;

namespace Maja.Compiler.Syntax;

public sealed record StatementReturnSyntax: StatementSyntax
{
    public ExpressionSyntax? Expression
        => Children.OfType<ExpressionSyntax>().SingleOrDefault();
}
