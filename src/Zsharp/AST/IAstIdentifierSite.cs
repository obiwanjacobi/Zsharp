using System;

namespace Zsharp.AST
{
    public interface IAstIdentifierSite
    {
        AstIdentifier? Identifier { get; }
        bool TrySetIdentifier(AstIdentifier identifier);
        void SetIdentifier(AstIdentifier identifier);

        public void ThrowIfIdentifierNotSet()
            => _ = Identifier ?? throw new InvalidOperationException("Identifier is not set.");
    }
}
