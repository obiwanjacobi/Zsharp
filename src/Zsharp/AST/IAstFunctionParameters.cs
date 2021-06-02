using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstFunctionParameters<T>
        where T : AstFunctionParameter
    {
        IEnumerable<T> Parameters { get; }
        bool TryAddParameter(T param);
    }

    public static class AstFunctionParametersExtensions
    {
        public static void AddParameter<T>(this IAstFunctionParameters<T> functionParameters, T param)
            where T : AstFunctionParameter
        {
            if (!functionParameters.TryAddParameter(param))
                throw new InternalErrorException("Parameter was already set or null.");
        }
    }
}
