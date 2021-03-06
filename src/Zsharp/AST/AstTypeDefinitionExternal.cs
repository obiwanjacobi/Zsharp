﻿namespace Zsharp.AST
{
    public class AstTypeDefinitionExternal : AstTypeDefinitionWithFields,
        IAstExternalNameSite
    {
        public AstTypeDefinitionExternal(string @namespace, string typeName, AstTypeReference? baseType)
            : base(AstNodeKind.Type)
        {
            this.SetIdentifier(new AstIdentifier(typeName, AstIdentifierKind.Type));
            ExternalName = new AstExternalName(@namespace, typeName);
            if (baseType is not null)
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
