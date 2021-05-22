using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterReference : AstTemplateParameter,
        IAstTypeReferenceSite
    {
        public AstTemplateParameterReference(Template_param_useContext context)
            : base(context)
        { }

        public AstTemplateParameterReference(AstTypeReference typeReference)
            : base(null)
        {
            this.SetTypeReference(typeReference);
        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override bool TryResolve()
        {
            // TODO: We need parent template (ref) context and
            // check the template definition.
            throw new InternalErrorException(
                "Cannot resolve a single Template Parameter.");
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterReference(this);
    }
}