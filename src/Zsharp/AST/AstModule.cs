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
            : base(AstNodeKind.Module)
        {
            Locality = locality;
        }

        public AstModuleLocality Locality { get; private set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);
    }
}