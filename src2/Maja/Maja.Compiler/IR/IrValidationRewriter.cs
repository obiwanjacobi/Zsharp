namespace Maja.Compiler.IR;

internal sealed class IrValidationRewriter : IrRewriter
{
    public bool Validate(IrModule module)
    {
        RewriteModule(module);
        return !Diagnostics.HasDiagnostics;
    }
}
