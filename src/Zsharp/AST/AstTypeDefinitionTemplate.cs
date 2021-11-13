using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinitionTemplate : AstTypeDefinition,
        IAstTemplateSite<AstTemplateParameterDefinition>,
        IAstGenericSite<AstGenericParameterDefinition>
    {
        protected AstTypeDefinitionTemplate(AstNodeKind nodeKind = AstNodeKind.Type)
            : base(nodeKind)
        { }

        private readonly List<AstTemplateParameter> _parameterList = new();
        public T TemplateParameterAt<T>(int index) where T : AstTemplateParameter
            => (T)_parameterList[index];

        // true when type is a template/generic definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register template parameter.");
            if (templateParameter is null)
                return false;

            _parameterList.Add(templateParameter);
            _templateParameters.Add(templateParameter);
            templateParameter.SetParent(this);
            Identifier.SymbolName.SetParameterCounts(_templateParameters.Count, _genericParameters.Count);

            return true;
        }

        public bool IsGeneric => _genericParameters.Count > 0;

        private readonly List<AstGenericParameterDefinition> _genericParameters = new();
        public IEnumerable<AstGenericParameterDefinition> GenericParameters => _genericParameters;

        public bool TryAddGenericParameter(AstGenericParameterDefinition genericParameter)
        {
            Ast.Guard(Identifier, "Identifier not set - cannot register generic parameter.");
            if (genericParameter is null)
                return false;

            _parameterList.Add(genericParameter);
            _genericParameters.Add(genericParameter);
            genericParameter.SetParent(this);
            Identifier.SymbolName.SetParameterCounts(_templateParameters.Count, _genericParameters.Count);

            return true;
        }
    }
}
