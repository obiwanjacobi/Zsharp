using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStructField : AstTypeFieldDefinition,
        IAstTypeReferenceSite
    {
        public AstTypeDefinitionStructField()
        { }

        internal AstTypeDefinitionStructField(ParserRuleContext context)
            : base(context)
        { }

        public bool IsTemplate 
            => _typeReference?.IsTemplateParameter ?? false;

        private AstTypeReferenceType? _typeReference;
        public AstTypeReferenceType? TypeReference => _typeReference;
        AstTypeReference? IAstTypeReferenceSite.TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, (AstTypeReferenceType?)typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionStructField(this);

        public override void VisitChildren(AstVisitor visitor)
            => TypeReference?.Accept(visitor);
    }
}
