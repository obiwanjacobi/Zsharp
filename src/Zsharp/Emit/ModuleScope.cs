using Mono.Cecil;

namespace Zsharp.Emit
{
    public sealed class ModuleScope : Scope, ILocalStorageProvider
    {
        public ModuleScope(EmitContext emitContext, ClassBuilder classBuilder)
            : base(emitContext)
        {
            ClassBuilder = classBuilder;
        }

        public ClassBuilder ClassBuilder { get; }

        public void CreateSlot(string name, TypeReference typeReference)
        {
            ClassBuilder.AddField(name, typeReference);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ClassBuilder.Dispose();
        }
    }
}
