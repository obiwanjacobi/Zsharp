using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTemplateParameterArgument : AstTemplateParameter,
        IAstTypeReferenceSite
    {
        internal AstTemplateParameterArgument(ParserRuleContext context)
            : base(context)
        { }

        public AstTemplateParameterArgument(AstTypeReference typeReference)
        {
            this.SetTypeReference(typeReference);
        }

        public AstTemplateParameterArgument(AstTemplateParameterArgument argumentToCopy)
            : base(argumentToCopy)
        {
            this.SetTypeReference(argumentToCopy.TypeReference.MakeCopy());
        }

        public int OrderIndex { get; set; }

        public bool HasParameterDefinition => _parameterDefinition is not null;

        private AstTemplateParameter? _parameterDefinition;
        // refers to the corresponding template/generic parameter definition
        public AstTemplateParameter ParameterDefinition
            => _parameterDefinition ?? throw new InternalErrorException("ParameterDefinition is not set.");

        public T? ParameterDefinitionAs<T>() where T : AstTemplateParameter
            => ParameterDefinition as T;

        public bool TrySetParameterDefinition(AstTemplateParameter templateOrGenericParameterDefinition)
            => Ast.SafeSet(ref _parameterDefinition, templateOrGenericParameterDefinition);

        public bool HasTypeReference => _typeReference is not null;

        private AstTypeReference? _typeReference;
        public AstTypeReference TypeReference
            => _typeReference ?? throw new InternalErrorException("TypeReference is not set.");

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterArgument(this);

        public override void VisitChildren(AstVisitor visitor)
            => _typeReference?.Accept(visitor);
    }
}
