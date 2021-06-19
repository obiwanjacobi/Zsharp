using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstGenericSite<T>
        where T : AstGenericParameter
    {
        bool IsGeneric { get; }
        IEnumerable<T> GenericParameters { get; }
        bool TryAddGenericParameter(T genericParameter);
    }

    public static class AstGenericSiteExtensions
    {
        public static void AddGenericParameter<T>(this IAstGenericSite<T> genericSite, T genericParameter)
            where T : AstGenericParameter
        {
            if (!genericSite.TryAddGenericParameter(genericParameter))
                throw new InternalErrorException(
                    "GenericParameter is already set or null.");
        }
    }
}