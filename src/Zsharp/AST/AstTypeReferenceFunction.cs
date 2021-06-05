﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public class AstTypeReferenceFunction : AstTypeReference,
        IAstTypeReferenceSite,
        IAstFunctionParameters<AstFunctionParameterReference>
    {
        public AstTypeReferenceFunction(ParserRuleContext context)
            : base(context)
        {
            this.SetIdentifier(new AstIdentifier(String.Empty, AstIdentifierType.Type));
        }

        private readonly List<AstFunctionParameterReference> _parameters = new();
        public IEnumerable<AstFunctionParameterReference> Parameters => _parameters;

        public bool TryAddParameter(AstFunctionParameterReference param)
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

        public string OverloadKey =>
            String.Join(String.Empty, _parameters.Select(p => p.TypeReference?.Identifier?.CanonicalName));

        private AstTypeReference? _typeReference;
        public AstTypeReference? TypeReference => _typeReference;

        public bool TrySetTypeReference(AstTypeReference? typeReference)
            => this.SafeSetParent(ref _typeReference, typeReference);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeReferenceFunction(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);

            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }

            TypeReference?.Accept(visitor);
        }

        public override string? ToString()
        {
            var txt = new StringBuilder();

            //txt.Append(Identifier!.Name);
            txt.Append(": (");

            for (int i = 0; i < Parameters.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Parameters.ElementAt(i);
                txt.Append(p.Expression?.TypeReference?.Identifier?.Name);
            }
            txt.Append(")");

            if (TypeReference?.Identifier != null)
            {
                txt.Append(": ");
                txt.Append(TypeReference.Identifier.Name);
            }

            return txt.ToString();
        }
    }
}
