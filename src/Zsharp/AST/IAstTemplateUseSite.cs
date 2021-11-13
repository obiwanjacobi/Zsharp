using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstTemplateUseSite<T>
        where T : AstTemplateParameterArgument
    {
        bool IsTemplateOrGeneric { get; }
        IEnumerable<T> TemplateArguments { get; }
        bool TryAddTemplateArgument(T templateArgument);
    }

    public static class AstTemplateUseSiteExtensions
    {
        public static void AddTemplateArgument<T>(this IAstTemplateUseSite<T> templateUseSite, T templateArgument)
            where T : AstTemplateParameterArgument
        {
            if (!templateUseSite.TryAddTemplateArgument(templateArgument))
                throw new InternalErrorException(
                    "TemplateArgument is already set or null.");
        }
    }
}
