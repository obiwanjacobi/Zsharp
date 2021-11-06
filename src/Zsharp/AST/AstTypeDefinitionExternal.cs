namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinitionWithFields,
        IAstExternalNameSite
    {
        public AstTypeDefinitionExternal(string typeName, AstTypeReference? baseType)
            : base(AstNodeKind.Type)
        {
            var symbolName = AstSymbolName.Parse(typeName, AstNameKind.External);
            this.SetIdentifier(new AstIdentifier(symbolName, AstIdentifierKind.Type));
            if (baseType is not null)
                SetBaseType(baseType);
        }

        public override bool IsExternal => true;

        public AstName ExternalName => Identifier.SymbolName.NativeName;

        public override void Accept(AstVisitor visitor)
        {
            // external types are not visited
        }
    }
}
