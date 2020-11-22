namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinition
    {
        public AstTypeDefinitionExternal(string typeName, AstTypeReference? baseType)
            : base(new AstIdentifierExternal(typeName, AstIdentifierType.Type))
        {
            if (baseType != null)
                SetBaseType(baseType);
        }

        public override bool IsExternal => true;

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeDefinitionExternal(this);
        }
    }
}
