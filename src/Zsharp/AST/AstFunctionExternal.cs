using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstFunctionExternal : AstFunctionDefinition,
        IAstExternalNameSite
    {
        public AstFunctionExternal(MethodDefinition method, bool hasSelfParameter)
        {
            MethodDefinition = method;
            HasSelfParameter = hasSelfParameter;
            ExternalName = new AstExternalName(
                method.DeclaringType.Namespace, method.Name, method.DeclaringType.Name);
        }

        internal MethodDefinition MethodDefinition { get; }

        public override bool IsExternal => true;

        public bool HasSelfParameter { get; }

        public AstExternalName ExternalName { get; }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }
    }
}