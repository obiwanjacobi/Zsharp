using System.Collections.Generic;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrStatementAssignment : IrStatement, IrContainer
{
    // used for generated code (no syntax)
    internal IrStatementAssignment(VariableSymbol symbol, IrExpression expression, IrLocality locality)
        : base(locality)
    {
        Symbol = symbol;
        Expression = expression;
    }
    public IrStatementAssignment(StatementAssignmentSyntax syntax, VariableSymbol symbol, IrExpression expression, IrLocality locality)
        : base(syntax, locality)
    {
        Symbol = symbol;
        Expression = expression;
    }

    public VariableSymbol Symbol { get; }
    public IrExpression Expression { get; }

    public new StatementAssignmentSyntax Syntax
        => (StatementAssignmentSyntax)base.Syntax;

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Expression.GetDescendentsOfType<T>();
}
