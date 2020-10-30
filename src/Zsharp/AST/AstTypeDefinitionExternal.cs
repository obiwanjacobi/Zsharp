namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinition
    {
        public AstTypeDefinitionExternal(string typeName, AstTypeReference? baseType)
            : base(new AstIdentifierExternal(typeName, AstIdentifierType.Type))
        {
            BaseType = baseType;
        }

        public override bool IsExternal => true;
    }
}
