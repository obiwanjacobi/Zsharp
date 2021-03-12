namespace Zsharp.AST
{
    public class AstTypeArrayOfT : AstTypeDefinition
    {
        AstTypeArrayOfT()
            : base(AstNodeType.Type)
        { }

        public override void Accept(AstVisitor visitor)
        {
            throw new System.NotImplementedException();
        }

        public void Instantiate(AstTypeReference typeReference)
        {

        }
    }

    public static class AstTypeArrayFunctions
    {
        public static void AddAll(AstSymbolTable symbols)
        {
            var construct = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.Array,
                AstTypeDefinitionIntrinsic.Array);
            construct.AddParameter("capacity", AstTypeDefinitionIntrinsic.U32);
            construct.AddTemplateParameter(AstTemplateParameterDefinitionIntrinsic.T);
            construct.CreateSymbols(symbols);
        }
    }
}
