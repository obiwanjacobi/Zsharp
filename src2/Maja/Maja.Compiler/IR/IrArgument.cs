using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrArgument : IrNode
{
    public IrArgument(ArgumentSyntax syntax,
        IrExpression expression, VariableSymbol? symbol)
        : base(syntax)
    {
        Expression = expression;
        Symbol = symbol;
    }

    public IrExpression Expression { get; }
    public VariableSymbol? Symbol { get; }
}