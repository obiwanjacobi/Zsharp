using Mono.Cecil;

namespace Zsharp.EmitIL
{
    public sealed class FunctionScope : Scope
    {
        public FunctionScope(EmitContext emitContext, MethodDefinition methodDefinition)
            : base(emitContext)
        {
            InstructionFactory = new InstructionFactory(methodDefinition.Body.GetILProcessor());
            CodeBuilder = new CodeBuilder(methodDefinition);
        }

        public InstructionFactory InstructionFactory { get; }

        public CodeBuilder CodeBuilder { get; }

        protected override void Dispose(bool disposing)
        {
            CodeBuilder.Apply();
            base.Dispose(disposing);
        }
    }
}
