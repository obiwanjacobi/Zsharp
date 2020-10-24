using System;

namespace Zsharp.AST
{
    public interface IAstIdentifierSite
    {
        AstIdentifier? Identifier { get; }
        bool TrySetIdentifier(AstIdentifier identifier);

        void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        public void ThrowIfIdentifierNotSet()
            => _ = Identifier ?? throw new InvalidOperationException("Identifier is not set.");
    }
}
