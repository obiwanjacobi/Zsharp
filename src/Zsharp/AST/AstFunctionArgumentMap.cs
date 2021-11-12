using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstFunctionArgumentMap
        : AstParameterArgumentMap<AstFunctionParameterDefinition, AstFunctionParameterArgument>
    {
        public AstFunctionArgumentMap(
            IEnumerable<AstFunctionParameterDefinition> parameters,
            IEnumerable<AstFunctionParameterArgument> arguments)
            : base(parameters, arguments)
        { }
    }
}
