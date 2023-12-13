using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrExpressionIdentifier : IrExpression
{
    public IrExpressionIdentifier(ExpressionIdentifierSyntax syntax, VariableSymbol symbol, TypeSymbol type)
        : base(syntax, type)
    {
        Symbol = symbol;
    }

    public VariableSymbol Symbol { get; }

    public bool IsDiscard
        => Symbol.Kind == SymbolKind.Discard;
}