using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionExternal : AstFunctionDefinition,
        IAstExternalNameSite
    {
        public AstFunctionDefinitionExternal(MethodDefinition method, bool hasSelfParameter)
            : base(new AstTypeDefinitionFunction())
        {
            MethodDefinition = method;
            HasSelfParameter = hasSelfParameter;
            ExternalName = new AstExternalName(
                method.DeclaringType.Namespace, method.Name, method.DeclaringType.Name);

            // TODO: built function type!
        }

        internal MethodDefinition MethodDefinition { get; }

        public override bool IsExternal => true;

        public bool HasSelfParameter { get; }

        public AstExternalName ExternalName { get; }
    }
}