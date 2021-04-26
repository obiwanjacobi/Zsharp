using System;

namespace Zsharp.AST
{
    public enum AstModuleLocality
    {
        Public,
        External
    }

    public abstract class AstModule : AstNode,
        IAstIdentifierSite
    {
        protected AstModule(AstModuleLocality locality)
            : base(AstNodeType.Module)
        {
            Locality = locality;
        }

        public AstModuleLocality Locality { get; private set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }
    }
}