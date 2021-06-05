using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinition : AstType,
        IAstTemplateSite<AstTemplateParameterDefinition>
    {
        protected AstTypeDefinition(AstNodeType nodeType = AstNodeType.Type)
            : base(nodeType)
        { }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public virtual bool IsUnsigned => false;

        public virtual bool IsStruct => false;

        // true when type is a template definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (templateParameter == null)
                return false;

            _templateParameters.Add(templateParameter);

            if (Identifier is not null)
                Identifier.TemplateParameterCount = _templateParameters.Count;

            return true;
        }
    }
}