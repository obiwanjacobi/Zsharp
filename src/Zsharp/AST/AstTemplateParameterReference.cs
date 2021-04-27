using System;
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
            SetTypeReference(typeReference);
        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

        public override bool TryResolve()
        {
            // TODO: We need parent template (ref) context and
            // check the template definition.
            throw new InvalidOperationException(
                "Cannot resolve a single Template Parameter.");
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterReference(this);
    }
}