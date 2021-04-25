namespace Zsharp.EmitCS
{
    public sealed class FunctionScope : Scope
    {
        private readonly CsBuilder? _csBuilder;

        public FunctionScope(EmitContext emitContext)
            : base(emitContext)
        {
            CodeBuilder = new CodeBuilder(emitContext.CsBuilder);
        }

        public FunctionScope(EmitContext emitContext, CsBuilder csBuilder)
            : base(emitContext)
        {
            _csBuilder = emitContext.CsBuilder;
            CodeBuilder = new CodeBuilder(csBuilder);
        }

        public CodeBuilder CodeBuilder { get; }

        protected override void Dispose(bool disposing)
        {
            CodeBuilder.Apply();
            if (_csBuilder != null)
            {
                _csBuilder.Append(CodeBuilder.ToString());
            }
            base.Dispose(disposing);
        }
    }
}
