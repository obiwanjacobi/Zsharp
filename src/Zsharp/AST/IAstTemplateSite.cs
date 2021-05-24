using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTemplateSite<T>
        where T : AstTemplateParameter
    {
        bool IsTemplate { get; }
        IEnumerable<T> TemplateParameters { get; }
        bool TryAddTemplateParameter(T templateParameter);
    }

    public static class AstTemplateSiteExtensions
    {
        public static void AddTemplateParameter<T>(this IAstTemplateSite<T> templateSite, T templateParameter)
            where T : AstTemplateParameter
        {
            if (!templateSite.TryAddTemplateParameter(templateParameter))
                throw new InternalErrorException(
                    "TemplateParameter is already set or null.");
        }
    }
}