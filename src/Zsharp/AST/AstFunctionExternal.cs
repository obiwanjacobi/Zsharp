using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstFunctionExternal : AstFunctionDefinition
    {
        public AstFunctionExternal(MethodDefinition method)
        {
            MethodDefinition = method;
            HasSelfParameter = !method.IsStatic;
        }

        public MethodDefinition MethodDefinition { get; }

        public bool HasSelfParameter { get; }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }
    }
}