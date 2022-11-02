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

        // can be null if no mod keyword was found
        public ModuleSyntax? ModuleSyntax
            => base.Syntax as ModuleSyntax;

        public ModuleSymbol Symbol { get; }
    }
}