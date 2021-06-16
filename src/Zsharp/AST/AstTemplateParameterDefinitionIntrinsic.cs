namespace Zsharp.AST
{
    public class AstTemplateParameterDefinitionIntrinsic : AstTemplateParameterDefinition
    {
        public AstTemplateParameterDefinitionIntrinsic(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        public override bool IsIntrinsic => true;
    }
}
