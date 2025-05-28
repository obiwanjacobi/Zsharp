namespace Maja.Compiler.IR;

internal sealed class IrValidationRewriter : IrRewriter
{
    public bool Validate(IrCompilation compilation)
    {
        RewriteCompilation(compilation);
        return !Diagnostics.HasDiagnostics;
    }
}
