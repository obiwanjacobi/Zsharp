using System;

namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool SetSymbol(AstSymbolEntry symbolEntry);

        public void ThrowIfSymbolNotSet()
            => _ = Symbol ?? throw new InvalidOperationException("Symbol is not set.");
    }
}