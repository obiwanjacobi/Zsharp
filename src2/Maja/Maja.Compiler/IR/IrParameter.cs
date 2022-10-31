using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrParameter : IrNode
{
    public IrParameter(ParameterSyntax syntax, ParameterSymbol symbol, IrType type)
        : base(syntax)
    {
        Symbol = symbol;
        Type = type;
    }

    public ParameterSymbol Symbol { get; }
    public IrType Type { get; }

    public new ParameterSyntax Syntax
        => (ParameterSyntax)base.Syntax;
}