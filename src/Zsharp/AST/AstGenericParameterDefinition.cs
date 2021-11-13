using System.Collections.Generic;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstGenericParameterDefinition : AstTemplateParameter,
        IAstIdentifierSite
    {
        public AstGenericParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        internal AstGenericParameterDefinition(ParserRuleContext context)
            : base(context)
        { }

        protected AstGenericParameterDefinition()
        { }

        public virtual bool IsIntrinsic => false;

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");

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
