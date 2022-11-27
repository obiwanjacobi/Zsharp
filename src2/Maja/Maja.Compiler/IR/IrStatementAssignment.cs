using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrStatementAssignment : IrStatement
{
    public IrStatementAssignment(StatementAssignmentSyntax syntax, VariableSymbol symbol, IrExpression expression)
        : base(syntax)
    {
        Symbol = symbol;
        Expression = expression;
    }

    public VariableSymbol Symbol { get; }
    public IrExpression Expression { get; }

    public new StatementAssignmentSyntax Syntax
        => (StatementAssignmentSyntax)base.Syntax;
}
