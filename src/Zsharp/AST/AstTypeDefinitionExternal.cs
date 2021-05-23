namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinition,
        IAstExternalNameSite
    {
        public AstTypeDefinitionExternal(string @namespace, string typeName, AstTypeReference? baseType)
            : base(AstNodeType.Type)
        {
            this.SetIdentifier(new AstIdentifier(typeName, AstIdentifierType.Type));
            ExternalName = new AstExternalName(@namespace, typeName);
            if (baseType != null)
                SetBaseType(baseType);
        }

        public override bool IsExternal => true;

        public AstExternalName ExternalName { get; }

        public override void Accept(AstVisitor visitor)
        {
            // external types are not visited
        }
    }
}
