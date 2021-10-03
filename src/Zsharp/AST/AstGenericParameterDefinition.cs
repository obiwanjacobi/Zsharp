using System.Collections.Generic;
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

        public AstTypeReference? DefaultType { get; internal set; }

        private readonly List<AstTypeReference> _constraintTypes = new();
        public IEnumerable<AstTypeReference> ConstraintTypes => _constraintTypes;

        public void AddConstraintType(AstTypeReference typeReference) => _constraintTypes.Add(typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitGenericParameterDefinition(this);
    }
}