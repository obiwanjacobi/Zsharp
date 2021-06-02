﻿using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstTypeDefinitionFunction : AstTypeDefinition,
        IAstTypeReferenceSite,
        IAstFunctionParameters<AstFunctionParameterDefinition>
    {
        public AstTypeDefinitionFunction()
        {

        }

        private readonly List<AstFunctionParameterDefinition> _parameters = new();
        public IEnumerable<AstFunctionParameterDefinition> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameterDefinition param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                // always make sure 'self' is first param
                if (param.Identifier == AstIdentifierIntrinsic.Self)
                    _parameters.Insert(0, param);
                else
                    _parameters.Add(param);
                return true;
            }
            return false;
        }

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeDefinitionFunction(this);
    }
}
