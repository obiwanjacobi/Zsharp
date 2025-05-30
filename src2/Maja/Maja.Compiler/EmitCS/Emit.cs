using Maja.Compiler.EmitCS.IR;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS;

internal static class Emit
{
    public static string EmitCode(IrProgram program)
    {
        var lowering = new IrCodeRewriter();
        var codeProg = lowering.CodeRewrite(program);

        var builder = new CodeBuilder();
        var ns = builder.OnProgram(codeProg);

        return builder.ToString();
    }
}
