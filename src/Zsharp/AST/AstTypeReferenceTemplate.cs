using Antlr4.Runtime;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeReferenceTemplate : AstTypeReference,
        IAstTemplateSite<AstTemplateParameterReference>
    {
        protected AstTypeReferenceTemplate(ParserRuleContext? context = null)
            : base(context)
        {
            _templateParameters = new();
        }

        protected AstTypeReferenceTemplate(AstTypeReference typeOrigin)
            : base(typeOrigin)
        { }

        public new AstTypeReferenceTemplate? TypeOrigin
            => (AstTypeReferenceTemplate?)base.TypeOrigin;

        private bool _isTemplateParameter;
        // true when type name is actually a template parameter name (T)
        public bool IsTemplateParameter
        {
            get { return TypeOrigin?.IsTemplateParameter ?? _isTemplateParameter; }
            set
            {
                if (IsProxy)
                    throw new InternalErrorException("Cannot set IsTemplateParameter property on a TypeReference proxy.");
                _isTemplateParameter = value;
            }
        }

        // true when type is a template instantiation
        public bool IsTemplate
            => TypeOrigin?.IsTemplate ?? _templateParameters!.Count > 0;

        private readonly List<AstTemplateParameterReference>? _templateParameters;
        public IEnumerable<AstTemplateParameterReference> TemplateParameters
            => TypeOrigin?.TemplateParameters ?? _templateParameters!;

        public bool TryAddTemplateParameter(AstTemplateParameterReference templateParameter)
        {
            if (TypeOrigin is not null || _templateParameters is null)
                throw new InternalErrorException(
                    "Cannot add Template Parameter onto a TypeReference Proxy.");

            if (templateParameter is AstTemplateParameterReference parameter)
            {
                _templateParameters.Add(parameter);

                if (Identifier is not null)
                    Identifier.SymbolName.AddTemplateParameter(parameter.TypeReference?.Identifier?.Name);
                return true;
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            if (_templateParameters is not null)
            {
                foreach (var param in _templateParameters)
                {
                    param.Accept(visitor);
                }
            }
        }
    }
}