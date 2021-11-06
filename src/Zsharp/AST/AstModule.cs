namespace Zsharp.AST
{
    public enum AstModuleLocality
    {
        Local,
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

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.Module, "Identifier must be of kind Module");
            return Ast.SafeSet(ref _identifier, identifier);
        }
    }
}