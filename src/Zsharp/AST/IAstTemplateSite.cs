using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTemplateSite
    {
        IEnumerable<AstTemplateParameter> Parameters { get; }
        bool TryAddTemplateParameter(AstTemplateParameter templateParameter);
        void AddTemplateParameter(AstTemplateParameter templateParameter);
    }
}