using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrExpressionIdentifier : IrExpression
{
    internal IrExpressionIdentifier(DeclaredVariableSymbol symbol, TypeSymbol type)
        : base(type)
    {
        Symbol = symbol;
    }
    public IrExpressionIdentifier(ExpressionIdentifierSyntax syntax, DeclaredVariableSymbol symbol, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
    }

    public DeclaredVariableSymbol Symbol { get; }

    public new ExpressionIdentifierSyntax Syntax
        => (ExpressionIdentifierSyntax)base.Syntax;
}
