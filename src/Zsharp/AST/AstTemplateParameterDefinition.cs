using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter,
        IAstIdentifierSite
    {
        public AstTemplateParameterDefinition(Template_param_anyContext context)
            : base(context)
        { }

        public AstTemplateParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        protected AstTemplateParameterDefinition()
        { }

        public virtual bool IsIntrinsic => false;

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.TemplateParameter, "Identifier must be of kind TemplateParameter");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterDefinition(this);
    }
}