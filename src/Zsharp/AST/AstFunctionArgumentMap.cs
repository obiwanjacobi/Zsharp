using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstFunctionArgumentMap
        : AstParameterArgumentMap<AstFunctionParameterDefinition, AstFunctionParameterReference>
    {
        public AstFunctionArgumentMap(
            IEnumerable<AstFunctionParameterDefinition> parameters,
            IEnumerable<AstFunctionParameterReference> arguments)
            : base(parameters, arguments)
        { }
    }
}
