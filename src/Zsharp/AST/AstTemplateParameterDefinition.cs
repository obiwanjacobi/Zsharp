using System.Collections.Generic;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter
    {
        internal AstTemplateParameterDefinition(ParserRuleContext context)
            : base(context)
        { }

        public AstTemplateParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        public AstTypeReference? DefaultType { get; internal set; }

        private readonly List<AstTypeReference> _constraintTypes = new();
        public IEnumerable<AstTypeReference> ConstraintTypes => _constraintTypes;

        public void AddConstraintType(AstTypeReference typeReference) => _constraintTypes.Add(typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterDefinition(this);
    }
}
