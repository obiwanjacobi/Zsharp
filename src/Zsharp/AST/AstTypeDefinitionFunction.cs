namespace Zsharp.AST
{
    public class AstTypeDefinitionFunction : AstTypeDefinition,
        IAstTypeReferenceSite
    {
        public AstTypeDefinitionFunction()
        {

        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionFunction(this);
    }
}
