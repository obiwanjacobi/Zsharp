namespace Zsharp.EmitIL
{
    public sealed class ModuleScope : Scope
    {
        public ModuleScope(EmitContext emitContext, ClassBuilder classBuilder)
            : base(emitContext)
        {
            ClassBuilder = classBuilder;
        }

        public ClassBuilder ClassBuilder { get; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ClassBuilder.Dispose();
        }
    }
}
