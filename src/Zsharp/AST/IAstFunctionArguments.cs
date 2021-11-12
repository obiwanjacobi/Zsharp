using System.Collections.Generic;

namespace Zsharp.AST
{
    public interface IAstFunctionArguments<T>
        where T : AstFunctionParameterArgument
    {
        IEnumerable<T> Arguments { get; }
        bool TryAddArgument(T param);
    }

    public static class AstFunctionArgumentsExtensions
    {
        public static void AddArgument<T>(this IAstFunctionArguments<T> functionArguments, T param)
            where T : AstFunctionParameterArgument
        {
            if (!functionArguments.TryAddArgument(param))
                throw new InternalErrorException("Argument was already set or null.");
        }
    }
}
