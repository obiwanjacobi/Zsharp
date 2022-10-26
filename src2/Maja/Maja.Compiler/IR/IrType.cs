using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR
{
    internal sealed class IrType : IrNode
    {
        public IrType(SyntaxNode syntax, TypeSymbol symbol)
            : base(syntax)
        {
            Symbol = symbol;
        }

        public TypeSymbol Symbol { get; }
    }
}