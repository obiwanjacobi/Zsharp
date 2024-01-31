using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrStatementReturn : IrStatement
{
    public IrStatementReturn(StatementReturnSyntax syntax, IrExpression? expression)
        : base(syntax, IrLocality.None)
    {
        Expression = expression;
    }

    public new StatementReturnSyntax Syntax
        => (StatementReturnSyntax)base.Syntax;

    public IrExpression? Expression { get; }
}