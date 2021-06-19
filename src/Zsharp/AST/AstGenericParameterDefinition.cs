using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstGenericParameterDefinition : AstGenericParameter,
        IAstIdentifierSite
    {
        public AstGenericParameterDefinition(Template_param_anyContext context)
            : base(context)
        { }

        public AstGenericParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        protected AstGenericParameterDefinition()
        { }

        public virtual bool IsIntrinsic => false;

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.TemplateParameter, "Identifier must be of kind GenericParameter");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitGenericParameterDefinition(this);
    }
}