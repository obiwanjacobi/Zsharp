using System.Collections.Generic;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrStatementAssignment : IrStatement, IrContainer
{
    // used for generated code (no syntax)
    internal IrStatementAssignment(DeclaredVariableSymbol symbol,
        IrOperatorAssignment assignmentOperator, IrExpression expression, IrLocality locality)
        : base(locality)
    {
        Symbol = symbol;
        AssignmentOperator = assignmentOperator;
        Expression = expression;
    }
    public IrStatementAssignment(StatementAssignmentSyntax syntax, DeclaredVariableSymbol symbol,
        IrOperatorAssignment assignmentOperator, IrExpression expression, IrLocality locality)
        : base(syntax, locality)
    {
        Symbol = symbol;
        AssignmentOperator = assignmentOperator;
        Expression = expression;
    }

    public DeclaredVariableSymbol Symbol { get; }
    public IrOperatorAssignment AssignmentOperator { get; }
    public IrExpression Expression { get; }

    public new StatementAssignmentSyntax Syntax
        => (StatementAssignmentSyntax)base.Syntax;

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Expression.GetDescendantsOfType<T>();
}
