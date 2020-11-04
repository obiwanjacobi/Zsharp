using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstFunctionExternal : AstFunctionParameters
    {
        public AstFunctionExternal(MethodDefinition method)
        {
            HasSelfParameter = !method.IsStatic;
        }

        public bool HasSelfParameter { get; }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }
    }
}