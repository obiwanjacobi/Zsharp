using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTemplateSite
    {
        bool IsTemplate { get; }
        IEnumerable<AstTemplateParameter> TemplateParameters { get; }
        bool TryAddTemplateParameter(AstTemplateParameter? templateParameter);
    }

    public static class AstTemplateSiteExtensions
    {
        public static void AddTemplateParameter(this IAstTemplateSite templateSite, AstTemplateParameter templateParameter)
        {
            if (!templateSite.TryAddTemplateParameter(templateParameter))
                throw new ZsharpException(
                    "TemplateParameter is already set or null.");
        }
    }
}