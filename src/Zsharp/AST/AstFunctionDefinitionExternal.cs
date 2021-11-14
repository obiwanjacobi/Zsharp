
using Zsharp.External.Metadata;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionExternal : AstFunctionDefinition
    {
        public AstFunctionDefinitionExternal(MethodMetadata method, bool hasSelfParameter)
            : base(new AstTypeDefinitionFunction())
        {
            MethodDefinition = method;
            HasSelfParameter = hasSelfParameter;
        }

        internal MethodMetadata MethodDefinition { get; }

        public override bool IsExternal => true;

        public bool HasSelfParameter { get; }
    }
}
