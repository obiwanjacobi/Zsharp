using Mono.Cecil;

namespace Zsharp.Emit
{
    public sealed class FunctionScope : Scope, ILocalStorageProvider
    {
        public FunctionScope(EmitContext emitContext, MethodDefinition methodDefinition)
            : base(emitContext)
        {
            InstructionFactory = new InstructionFactory(methodDefinition.Body.GetILProcessor());
            CodeBuilder = new CodeBuilder(methodDefinition);
        }

        public InstructionFactory InstructionFactory { get; }

        public CodeBuilder CodeBuilder { get; }

        public void CreateSlot(string name, TypeReference typeReference)
        {
            CodeBuilder.AddVariable(name, typeReference);
        }

        protected override void Dispose(bool disposing)
        {
            CodeBuilder.Apply();
            base.Dispose(disposing);
        }
    }
}
