using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTemplateSite
    {
        bool IsTemplate { get; }
        IEnumerable<AstTemplateParameter> TemplateParameters { get; }
        bool TryAddTemplateParameter(AstTemplateParameter? templateParameter);
        void AddTemplateParameter(AstTemplateParameter templateParameter);
    }
}