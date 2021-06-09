﻿using System.Collections.Generic;

namespace Zsharp.AST
{
    public abstract class AstTypeDefinitionTemplate : AstTypeDefinition,
        IAstTemplateSite<AstTemplateParameterDefinition>
    {
        protected AstTypeDefinitionTemplate(AstNodeType nodeType = AstNodeType.Type)
            : base(nodeType)
        { }

        // true when type is a template definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameterDefinition> _templateParameters = new();
        public IEnumerable<AstTemplateParameterDefinition> TemplateParameters => _templateParameters;

        public virtual bool TryAddTemplateParameter(AstTemplateParameterDefinition templateParameter)
        {
            if (templateParameter is null)
                return false;

            _templateParameters.Add(templateParameter);

            Ast.Guard(Identifier, "Identifier not set - cannot register template parameter.");
            Identifier!.SymbolName.SetTemplateParameterCount(_templateParameters.Count);

            return true;
        }
    }
}