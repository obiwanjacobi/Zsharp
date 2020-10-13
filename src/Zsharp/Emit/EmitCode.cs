using Mono.Cecil;
using Zsharp.AST;

namespace Zsharp.Generation
{
    public class EmitCode : AstVisitor
    {
        public EmitContext Context { get; } = new EmitContext();

        public override void VisitFunction(AstFunction function)
        {

            var fn = new MethodDefinition(function.Identifier.Name,
                ToMethodAttibutes(function), ToTypeReference(function.TypeReference));
        }

        private TypeReference ToTypeReference(AstTypeReference typeReference)
        {
            // TODO:
            IMetadataScope metadataScope = null;
            var typeRef = new TypeReference("Todo",
                typeReference.TypeDefinition.Identifier.Name,
                Context.Module,
                metadataScope, true);

            return typeRef;
        }

        private MethodAttributes ToMethodAttibutes(AstFunction function)
        {
            var attrs = MethodAttributes.Static | MethodAttributes.HideBySig;

            attrs |= (function.Symbol.SymbolLocality == AstSymbolLocality.Exported)
                ? MethodAttributes.Public : MethodAttributes.Private;

            return attrs;
        }
    }
}
