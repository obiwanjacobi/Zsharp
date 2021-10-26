
using Zsharp.External.Metadata;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionExternal : AstFunctionDefinition,
        IAstExternalNameSite
    {
        public AstFunctionDefinitionExternal(MethodMetadata method, bool hasSelfParameter)
            : base(new AstTypeDefinitionFunction())
        {
            var declType = method.GetDeclaringType();

            MethodDefinition = method;
            HasSelfParameter = hasSelfParameter;
            ExternalName = new AstName(
                hasSelfParameter 
                    ? declType.Namespace
                    : $"{declType.Namespace}.{declType.Name}",
                method.Name, AstNameKind.External);

            // TODO: built function type!
        }

        internal MethodMetadata MethodDefinition { get; }

        public override bool IsExternal => true;

        public bool HasSelfParameter { get; }

        public AstName ExternalName { get; }
    }
}