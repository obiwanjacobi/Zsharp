using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstGenericUseSite<T>
        where T : AstGenericParameterArgument
    {
        bool IsGeneric { get; }
        IEnumerable<T> GenericArguments { get; }
        bool TryAddGenericArgument(T genericParameter);
    }

    public static class AstGenericUseSiteExtensions
    {
        public static void AddGenericArgument<T>(this IAstGenericUseSite<T> genericSite, T genericArgument)
            where T : AstGenericParameterArgument
        {
            if (!genericSite.TryAddGenericArgument(genericArgument))
                throw new InternalErrorException(
                    "GenericArgument is already set or null.");
        }
    }
}
