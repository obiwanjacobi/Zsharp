﻿using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstTemplateInstanceType : AstTypeDefinition
    {
        public AstTemplateInstanceType(AstTypeDefinitionIntrinsic intrinsicTypeDef)
        {
            TypeDefinition = intrinsicTypeDef;
        }

        public AstTypeDefinitionIntrinsic TypeDefinition { get; }

        private readonly List<AstTypeReference> _templateParameters = new();
        public IEnumerable<AstTypeReference> TemplateParameters => _templateParameters;

        private void AddTemplateParameter(AstTypeReference typeReference)
        {
            var typeRef = typeReference.MakeCopy();
            typeRef.SetParent(this);
            _templateParameters.Add(typeRef);
        }

        public void Instantiate(AstTypeReferenceType type)
        {
            Context = type.Context;
            this.SetIdentifier(type.Identifier!.MakeCopy());

            foreach (AstTemplateParameterReference templateParam in type.TemplateParameters)
            {
                AddTemplateParameter(templateParam.TypeReference!);
            }
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateInstanceType(this);
    }
}
