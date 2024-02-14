using Maja.Compiler.Eval;

namespace Maja.UnitTests.Compiler.Eval;

internal static class Eval
{
    public static EvaluatorResult Run(string code, bool allowError = false)
    {
        var eval = new Evaluator();
        var result = eval.Eval(code);

        if (!allowError)
        {
            result.Diagnostics.Should().BeEmpty();
        }

        return result;
    }
}
