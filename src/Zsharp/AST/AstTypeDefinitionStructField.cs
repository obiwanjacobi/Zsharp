using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionStructField : AstTypeFieldDefinition,
        IAstTypeReferenceSite
    {
        public AstTypeDefinitionStructField()
        { }

        public AstTypeDefinitionStructField(Struct_field_defContext context)
            : base(context)
        { }

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
