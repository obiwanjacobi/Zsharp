using System;

namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool TrySetSymbol(AstSymbolEntry symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException(
                    "SymbolEntry is already set or null.");
        }

        public void ThrowIfSymbolNotSet()
            => _ = Symbol ?? throw new InvalidOperationException("Symbol is not set.");
    }
}