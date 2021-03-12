namespace Zsharp.AST
{
    public class AstTemplateParameterDefinitionIntrinsic : AstTemplateParameterDefinition
    {
        public readonly static AstTemplateParameterDefinitionIntrinsic T = new(AstIdentifierIntrinsic.T);

        public AstTemplateParameterDefinitionIntrinsic(AstIdentifier identifier)
        {
            SetIdentifier(identifier);
        }

        public override bool IsIntrinsic => true;
    }
}
