using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExpressionIdentifier : IrExpression
{
    public IrExpressionIdentifier(ExpressionIdentifierSyntax syntax, VariableSymbol symbol, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
    }

    public VariableSymbol Symbol { get; }

    public new ExpressionIdentifierSyntax Syntax
        => (ExpressionIdentifierSyntax)base.Syntax;
}
