using Maja.Compiler.EmitCS.IR;
using Maja.Compiler.IR;

namespace Maja.Compiler.EmitCS;

internal static class Emit
{
    public static string EmitCode(IrProgram program)
    {
        var lowering = new IrCodeRewriter();
        var programs = lowering.CodeRewrite(program);

        var builder = new CodeBuilder();

        foreach (var prog in programs)
        {
            var ns = builder.OnProgram(prog);
        }

        return builder.ToString();
    }
}
