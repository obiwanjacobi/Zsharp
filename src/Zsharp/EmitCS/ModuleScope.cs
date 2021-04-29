namespace Zsharp.EmitCS
{
    public sealed class ModuleScope : Scope
    {
        internal ModuleScope(EmitContext emitContext, ClassBuilder classBuilder)
            : base(emitContext)
        {
            ClassBuilder = classBuilder;
        }

        internal ClassBuilder ClassBuilder { get; }
    }
}
