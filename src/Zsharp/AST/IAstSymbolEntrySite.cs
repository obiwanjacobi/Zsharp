﻿using System;

namespace Zsharp.AST
{
    public interface IAstSymbolEntrySite
    {
        AstSymbolEntry? Symbol { get; }
        bool TrySetSymbol(AstSymbolEntry symbolEntry);
        void SetSymbol(AstSymbolEntry symbolEntry);
        bool TryResolve();

        public void ThrowIfSymbolNotSet()
            => _ = Symbol ?? throw new InvalidOperationException("Symbol is not set.");
    }
}