using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR
{
    internal sealed class IrModule : IrNode
    {
        public IrModule(SyntaxNode syntax, ModuleSymbol symbol)
            : base(syntax)
        {
            Symbol = symbol;
        }

        public ModuleSymbol Symbol { get; }
    }
}