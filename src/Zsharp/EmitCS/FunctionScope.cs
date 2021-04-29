namespace Zsharp.EmitCS
{
    public sealed class FunctionScope : Scope
    {
        internal FunctionScope(EmitContext emitContext, CsBuilder csBuilder)
            : base(emitContext)
        {
            CodeBuilder = new CodeBuilder(csBuilder);
        }

        internal CodeBuilder CodeBuilder { get; }
    }
}
