using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstGenericSite<T>
        where T : AstGenericParameterDefinition
    {
        bool IsGeneric { get; }
        IEnumerable<T> GenericParameters { get; }
        bool TryAddGenericParameter(T genericParameter);
    }

    public static class AstGenericSiteExtensions
    {
        public static void AddGenericParameter<T>(this IAstGenericSite<T> genericSite, T genericParameter)
            where T : AstGenericParameterDefinition
        {
            if (!genericSite.TryAddGenericParameter(genericParameter))
                throw new InternalErrorException(
                    "GenericParameter is already set or null.");
        }
    }
}
