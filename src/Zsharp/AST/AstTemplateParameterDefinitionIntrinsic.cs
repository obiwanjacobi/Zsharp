namespace Zsharp.AST
{
    public class AstTemplateParameterDefinitionIntrinsic : AstTemplateParameterDefinition
    {
        public readonly static AstTemplateParameterDefinitionIntrinsic T = new(AstIdentifierIntrinsic.T);

        public AstTemplateParameterDefinitionIntrinsic(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        public override bool IsIntrinsic => true;
    }
}
