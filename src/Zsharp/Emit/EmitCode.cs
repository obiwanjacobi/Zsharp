using Mono.Cecil;
using Mono.Cecil.Cil;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = EmitContext.Create(assemblyName);
        }

        public EmitContext? Context { get; private set; }

        public override void VisitModule(AstModule module)
        {
            if (Context == null)
            {
                Context = EmitContext.Create(module.Name);
            }

            using var scope = Context.AddModuleClass(module);

            VisitChildren(module);
        }

        public override void VisitFunction(AstFunction function)
        {
            var fn = new MethodDefinition(
                function.Identifier.Name,
                ToMethodAttibutes(function),
                ToTypeReference(function.TypeReference)
            );

            using var scope = Context.Add(fn);

            VisitChildren(function);
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            if (branch.BranchType == AstBranchType.ExitFunction)
            {
                if (branch.HasExpression)
                {
                    VisitChildren(branch);
                    // TODO: ld expression result
                }
                Context.ILProcessor.Append(Context.ILProcessor.Create(OpCodes.Ret));
            }
        }

        private TypeReference ToTypeReference(AstTypeReference typeReference)
        {
            if (typeReference == null)
            {
                // TODO: Replace with Zsharp.Void
                return Context.Module.TypeSystem.Void;
            }

            if (typeReference.TypeDefinition.IsIntrinsic)
            {
                // Map intrinsic data types to .NET data types
                var type = ((AstTypeIntrinsic)typeReference.TypeDefinition).SystemType;
                return Context.Module.ImportReference(type);
            }

            // TODO: what does this do?
            IMetadataScope? metadataScope = null;

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
