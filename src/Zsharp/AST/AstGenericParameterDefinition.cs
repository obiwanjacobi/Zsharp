using System.Collections.Generic;
using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstGenericParameterDefinition : AstTemplateParameter
    {
        public AstGenericParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        public AstGenericParameterDefinition(AstGenericParameterDefinition parameterToCopy)
            : base(parameterToCopy)
        {
            DefaultType = parameterToCopy.DefaultType;
            _constraintTypes.AddRange(parameterToCopy.ConstraintTypes);
        }

        internal AstGenericParameterDefinition(ParserRuleContext context)
            : base(context)
        { }

        protected AstGenericParameterDefinition()
        { }

        public virtual bool IsIntrinsic => false;

        public AstTypeReference? DefaultType { get; internal set; }

        private readonly List<AstTypeReference> _constraintTypes = new();
        public IEnumerable<AstTypeReference> ConstraintTypes => _constraintTypes;

        public void AddConstraintType(AstTypeReference typeReference) => _constraintTypes.Add(typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitGenericParameterDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);

            DefaultType?.Accept(visitor);
            foreach (var constraint in _constraintTypes)
            {
                constraint.Accept(visitor);
            }
        }
    }
}
