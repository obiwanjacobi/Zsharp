namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinitionWithFields,
        IAstExternalNameSite
    {
        public AstTypeDefinitionExternal(string @namespace, string typeName, AstTypeReference? baseType)
            : base(AstNodeKind.Type)
        {
            this.SetIdentifier(new AstIdentifier(typeName, AstIdentifierKind.Type));
            ExternalName = new AstName(@namespace, typeName, AstNameKind.External);
            if (baseType is not null)
                SetBaseType(baseType);
        }

        public override bool IsExternal => true;

        public AstName ExternalName { get; }

        public override void Accept(AstVisitor visitor)
        {
            // external types are not visited
        }
    }
}
